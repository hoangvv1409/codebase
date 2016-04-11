using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Instrumentation;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus.Instrumentation;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus.Utils;
using ADR.Mobile.Infrastructure.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus
{
    public class SessionSubscriptionReceiver : IMessageReceiver
    {
        private static readonly TimeSpan AcceptSessionLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly ServiceBusSettings settings;
        private readonly string topic;
        private readonly string subscription;
        private readonly bool requiresSequentialProcessing;
        private readonly object lockObject = new object();
        private readonly RetryStrategy retryStrategy;
        private readonly RetryPolicy receiveRetryPolicy;
        private readonly int maxRetryNumber;
        private readonly ISessionSubscriptionReceiverInstrumentation instrumentation;
        private readonly DynamicThrottling dynamicThrottling;
        private CancellationTokenSource cancellationSource;
        private SubscriptionClient client;

        public SessionSubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool requiresSequentialProcessing = true)
            : this(
                settings,
                topic,
                subscription,
                requiresSequentialProcessing,
                new SessionSubscriptionReceiverInstrumentation(subscription, false),
                10,
                new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1)))
        {
        }

        public SessionSubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool requiresSequentialProcessing, ISessionSubscriptionReceiverInstrumentation instrumentation)
            : this(
                settings,
                topic,
                subscription,
                requiresSequentialProcessing,
                instrumentation,
                10,
                new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1)))
        {
        }

        protected SessionSubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool requiresSequentialProcessing, ISessionSubscriptionReceiverInstrumentation instrumentation, int maxRetryNumber, RetryStrategy backgroundRetryStrategy)
        {
            this.settings = settings;
            this.topic = topic;
            this.subscription = subscription;
            this.requiresSequentialProcessing = requiresSequentialProcessing;
            this.instrumentation = instrumentation;

            var messagingFactory = MessagingFactory.CreateFromConnectionString(this.settings.ConnectionString);
            this.client = messagingFactory.CreateSubscriptionClient(topic, subscription);
            if (this.requiresSequentialProcessing)
            {
                this.client.PrefetchCount = 10;
            }
            else
            {
                this.client.PrefetchCount = 15;
            }

            this.dynamicThrottling =
                new DynamicThrottling(
                    maxDegreeOfParallelism: 500,
                    minDegreeOfParallelism: 30,
                    penaltyAmount: 3,
                    workFailedPenaltyAmount: 5,
                    workCompletedParallelismGain: 1,
                    intervalForRestoringDegreeOfParallelism: 10000);

            this.retryStrategy = backgroundRetryStrategy;
            this.maxRetryNumber = maxRetryNumber;

            this.receiveRetryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(backgroundRetryStrategy);
            this.receiveRetryPolicy.Retrying += (s, e) =>
            {
                this.dynamicThrottling.Penalize();
                Trace.TraceWarning(
                    "An error occurred in attempt number {1} to receive a message from subscription {2}: {0}",
                    e.LastException.Message,
                    e.CurrentRetryCount,
                    this.subscription);
            };
        }

        /// <summary>
        /// Handler for incoming messages. The return value indicates whether the message should be disposed.
        /// </summary>
        protected Func<BrokeredMessage, MessageReleaseAction> MessageHandler { get; private set; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start(Func<BrokeredMessage, MessageReleaseAction> messageHandler)
        {
            lock (this.lockObject)
            {
                // If it's not null, there is already a listening task.
                if (this.cancellationSource == null)
                {
                    this.MessageHandler = messageHandler;
                    this.cancellationSource = new CancellationTokenSource();
                    Task.Factory.StartNew(() => this.AcceptSession(this.cancellationSource.Token), this.cancellationSource.Token);
                    this.dynamicThrottling.Start(this.cancellationSource.Token);
                }
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Stop()
        {
            lock (this.lockObject)
            {
                using (this.cancellationSource)
                {
                    if (this.cancellationSource != null)
                    {
                        this.cancellationSource.Cancel();
                        this.cancellationSource = null;
                        this.MessageHandler = null;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the listener if it was started previously.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();

            if (disposing)
            {
                using (this.instrumentation as IDisposable) { }
                using (this.dynamicThrottling as IDisposable) { }
            }
        }

        protected virtual MessageReleaseAction InvokeMessageHandler(BrokeredMessage message)
        {
            return this.MessageHandler != null ? this.MessageHandler(message) : MessageReleaseAction.AbandonMessage;
        }

        ~SessionSubscriptionReceiver()
        {
            Dispose(false);
        }

        private void AcceptSession(CancellationToken cancellationToken)
        {
            this.dynamicThrottling.WaitUntilAllowedParallelism(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                // Initialize a custom action acting as a callback whenever a non-transient exception occurs while accepting a session.
                Action<AggregateException> recoverAcceptSession = ex =>
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        if (e is TimeoutException) { }
                        else
                        {
                            // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                            Trace.TraceError("An unrecoverable error occurred while trying to accept a session in subscription {1}:\r\n{0}", ex, this.subscription);
                            this.dynamicThrottling.Penalize();
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // Continue accepting new sessions until told to stop regardless of any exceptions.
                        TaskEx.Delay(10000).ContinueWith(t => AcceptSession(cancellationToken));
                    }
                };

                this.receiveRetryPolicy.ExecuteAsync(() => this.client.AcceptMessageSessionAsync(AcceptSessionLongPollingTimeout).ContinueWith(a =>
                {
                    if (a.Exception != null)
                    {
                        recoverAcceptSession.Invoke(a.Exception);
                    }
                    else
                    {
                        MessageSession session = a.Result;
                        if (session != null)
                        {
                            this.instrumentation.SessionStarted();
                            this.dynamicThrottling.NotifyWorkStarted();
                            // starts a new task to process new sessions in parallel when enough threads are available
                            Task.Factory.StartNew(() => this.AcceptSession(cancellationToken), cancellationToken);
                            this.ReceiveMessagesAndCloseSession(session, cancellationToken);
                        }
                        else
                        {
                            this.AcceptSession(cancellationToken);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// Receives the messages in an asynchronous loop and closes the session once there are no more messages.
        /// </summary>
        private void ReceiveMessagesAndCloseSession(MessageSession session, CancellationToken cancellationToken)
        {
            CountdownEvent unreleasedMessages = new CountdownEvent(1);

            Action<bool> closeSession = (bool success) =>
            {
                Action doClose = () =>
                {
                    try
                    {
                        unreleasedMessages.Signal();
                        if (!unreleasedMessages.Wait(15000, cancellationToken))
                        {
                            Trace.TraceWarning("Waited for pending unreleased messages before closing session in subscription {0} but they did not complete in time", this.subscription);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    finally
                    {
                        unreleasedMessages.Dispose();
                    }

                    this.receiveRetryPolicy.ExecuteAsync(() => session.CloseAsync().ContinueWith(s =>
                    {
                        if (s.Exception != null)
                        {
                            Exception ex = s.Exception;
                            this.instrumentation.SessionEnded();
                            Trace.TraceError("An unrecoverable error occurred while trying to close a session in subscription {1}:\r\n{0}", ex, this.subscription);
                            this.dynamicThrottling.NotifyWorkCompletedWithError();
                        }
                        else
                        {
                            this.instrumentation.SessionEnded();
                            if (success)
                            {
                                this.dynamicThrottling.NotifyWorkCompleted();
                            }
                            else
                            {
                                this.dynamicThrottling.NotifyWorkCompletedWithError();
                            }
                        }
                    }));
                };

                if (this.requiresSequentialProcessing)
                {
                    doClose.Invoke();
                }
                else
                {
                    // Allow some time for releasing the messages before closing. Also, continue in a non I/O completion thread in order to block.
                    TaskEx.Delay(200).ContinueWith(t => doClose());
                }
            };

            // Declare an action to receive the next message in the queue or closes the session if cancelled.
            Action receiveNext = null;

            // Declare an action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            Action<Exception> recoverReceive = null;

            // Declare an action responsible for the core operations in the message receive loop.
            Action receiveMessage = (() =>
            {
                // Use a retry policy to execute the Receive action in an asynchronous and reliable fashion.
                this.receiveRetryPolicy.ExecuteAsync(() => session.ReceiveAsync(TimeSpan.Zero).ContinueWith(m =>
                {
                    if (m.Exception != null)
                    {
                        recoverReceive.Invoke(m.Exception);
                    }
                    else
                    {
                        BrokeredMessage msg = m.Result;
                        if (msg != null)
                        {
                            var roundtripStopwatch = Stopwatch.StartNew();
                            long schedulingElapsedMilliseconds = 0;
                            long processingElapsedMilliseconds = 0;

                            unreleasedMessages.AddCount();

                            Task.Factory.StartNew(() =>
                            {
                                var releaseAction = MessageReleaseAction.AbandonMessage;

                                try
                                {
                                    this.instrumentation.MessageReceived();

                                    schedulingElapsedMilliseconds = roundtripStopwatch.ElapsedMilliseconds;

                                    // Make sure the process was told to stop receiving while it was waiting for a new message.
                                    if (!cancellationToken.IsCancellationRequested)
                                    {
                                        try
                                        {
                                            try
                                            {
                                                // Process the received message.
                                                releaseAction = this.InvokeMessageHandler(msg);

                                                processingElapsedMilliseconds = roundtripStopwatch.ElapsedMilliseconds - schedulingElapsedMilliseconds;
                                                this.instrumentation.MessageProcessed(releaseAction.Kind == MessageReleaseActionKind.Complete, processingElapsedMilliseconds);
                                            }
                                            catch
                                            {
                                                processingElapsedMilliseconds = roundtripStopwatch.ElapsedMilliseconds - schedulingElapsedMilliseconds;
                                                this.instrumentation.MessageProcessed(false, processingElapsedMilliseconds);

                                                throw;
                                            }
                                        }
                                        finally
                                        {
                                            if (roundtripStopwatch.Elapsed > TimeSpan.FromSeconds(45))
                                            {
                                                this.dynamicThrottling.Penalize();
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    // Ensure that any resources allocated by a BrokeredMessage instance are released.
                                    if (this.requiresSequentialProcessing)
                                    {
                                        this.ReleaseMessage(msg, releaseAction, () => { receiveNext(); }, () => { closeSession(false); }, unreleasedMessages, processingElapsedMilliseconds, schedulingElapsedMilliseconds, roundtripStopwatch);
                                    }
                                    else
                                    {
                                        // Receives next without waiting for the message to be released.
                                        this.ReleaseMessage(msg, releaseAction, () => { }, () => { this.dynamicThrottling.Penalize(); }, unreleasedMessages, processingElapsedMilliseconds, schedulingElapsedMilliseconds, roundtripStopwatch);
                                        receiveNext.Invoke();
                                    }
                                }
                            });
                        }
                        else
                        {
                            // no more messages in the session, close it and do not continue receiving
                            closeSession(true);
                        }
                    }
                }));
            });

            // Initialize an action to receive the next message in the queue or closes the session if cancelled.
            receiveNext = () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    // Continue receiving and processing new messages until told to stop.
                    receiveMessage.Invoke();
                }
                else
                {
                    closeSession(true);
                }
            };

            // Initialize a custom action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            recoverReceive = ex =>
            {
                // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                Trace.TraceError("An unrecoverable error occurred while trying to receive a new message from subscription {1}:\r\n{0}", ex, this.subscription);

                // Cannot continue to receive messages from this session.
                closeSession(false);
            };

            // Start receiving messages asynchronously for the session.
            receiveNext.Invoke();
        }

        private void ReleaseMessage(BrokeredMessage msg, MessageReleaseAction releaseAction, Action completeReceive, Action onReleaseError, CountdownEvent countdown, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            switch (releaseAction.Kind)
            {
                case MessageReleaseActionKind.Complete:
                    msg.SafeCompleteAsync(
                        this.subscription,
                        operationSucceeded =>
                        {
                            msg.Dispose();
                            this.OnMessageCompleted(operationSucceeded, countdown);
                            if (operationSucceeded)
                            {
                                completeReceive();
                            }
                            else
                            {
                                onReleaseError();
                            }
                        },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.Abandon:
                    this.dynamicThrottling.Penalize();
                    msg.SafeAbandonAsync(
                        this.subscription,
                        operationSucceeded =>
                        {
                            msg.Dispose();
                            this.OnMessageCompleted(false, countdown);

                            onReleaseError();
                        },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.DeadLetter:
                    this.dynamicThrottling.Penalize();
                    msg.SafeDeadLetterAsync(
                        this.subscription,
                        releaseAction.DeadLetterReason,
                        releaseAction.DeadLetterDescription,
                        operationSucceeded =>
                        {
                            msg.Dispose();
                            this.OnMessageCompleted(false, countdown);

                            if (operationSucceeded)
                            {
                                completeReceive();
                            }
                            else
                            {
                                onReleaseError();
                            }
                        },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                default:
                    break;
            }
        }

        private void OnMessageCompleted(bool success, CountdownEvent countdown)
        {
            this.instrumentation.MessageCompleted(success);
            try
            {
                countdown.Signal();
            }
            catch (ObjectDisposedException)
            {
                // It could happen in a rare case that due to a timing issue between closing the session and disposing the countdown,
                // that the countdown is already disposed. This is OK and it can continue processing normally.
            }
        }

        private void RetryWarning(RetryingEventArgs e)
        {
            this.dynamicThrottling.Penalize();
            Trace.TraceWarning(
                "An error occurred in attempt number {1} to receive a message from subscription {2}: {0}",
                e.LastException.Message,
                e.CurrentRetryCount,
                this.subscription);
        }
    }
}

