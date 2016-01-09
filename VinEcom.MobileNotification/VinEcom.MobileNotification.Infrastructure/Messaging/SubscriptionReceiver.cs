using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;
using VinEcom.MobileNotification.Infrastructure.Instrumentation;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;
using VinEcom.MobileNotification.Infrastructure.Util;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public class SubscriptionReceiver : IMessageReceiver
    {
        private static readonly TimeSpan ReceiveLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly Uri serviceUri;
        private readonly ServiceBusSettings settings;
        private readonly string topic;
        private readonly ISubscriptionReceiverInstrumentation instrumentation;
        private string subscription;
        private readonly object lockObject = new object();
        private readonly RetryPolicy receiveRetryPolicy;
        private readonly RetryStrategy retryStrategy;
        private readonly int maxNumberRetry;
        private readonly bool processInParallel;
        private readonly DynamicThrottling dynamicThrottling;
        private CancellationTokenSource cancellationSource;
        private SubscriptionClient client;

        public SubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool processInParallel = false)
            : this(
                settings,
                topic,
                subscription,
                processInParallel,
                new SubscriptionReceiverInstrumentation(subscription, false),
                10,
                new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1)))
        {
        }

        public SubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool processInParallel, ISubscriptionReceiverInstrumentation instrumentation)
            : this(
                settings,
                topic,
                subscription,
                processInParallel,
                instrumentation,
                10,
                new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1)))
        {
        }

        protected SubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool processInParallel, ISubscriptionReceiverInstrumentation instrumentation, int maxNumberRetry, RetryStrategy backgroundRetryStrategy)
        {
            this.settings = settings;
            this.topic = topic;
            this.subscription = subscription;
            this.processInParallel = processInParallel;
            this.instrumentation = instrumentation;

            var messagingFactory = MessagingFactory.CreateFromConnectionString(this.settings.ConnectionString);
            this.client = messagingFactory.CreateSubscriptionClient(topic, subscription);
            if (this.processInParallel)
            {
                this.client.PrefetchCount = 500;
            }
            else
            {
                this.client.PrefetchCount = 100;
            }

            this.dynamicThrottling =
                new DynamicThrottling(
                    maxDegreeOfParallelism: 10000,
                    minDegreeOfParallelism: 50,
                    penaltyAmount: 3,
                    workFailedPenaltyAmount: 5,
                    workCompletedParallelismGain: 1,
                    intervalForRestoringDegreeOfParallelism: 8000);

            this.retryStrategy = backgroundRetryStrategy;
            this.maxNumberRetry = maxNumberRetry;

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
                this.MessageHandler = messageHandler;
                this.cancellationSource = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                    this.ReceiveMessages(this.cancellationSource.Token),
                    this.cancellationSource.Token);
                this.dynamicThrottling.Start(this.cancellationSource.Token);
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

        ~SubscriptionReceiver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Receives the messages in an endless asynchronous loop.
        /// </summary>
        private void ReceiveMessages(CancellationToken cancellationToken)
        {
            // Declare an action to receive the next message in the queue or end if cancelled.
            Action receiveNext = null;

            // Declare an action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            Action<Exception> recoverReceive = null;

            // Declare an action responsible for the core operations in the message receive loop.
            Action receiveMessage = (() =>
            {
                this.receiveRetryPolicy.ExecuteAsync(() => this.client.ReceiveAsync(ReceiveLongPollingTimeout).ContinueWith(m =>
                {
                    if (m.Exception != null)
                    {
                        recoverReceive(m.Exception);
                    }
                    else
                    {
                        BrokeredMessage msg = m.Result;
                        // Process the message once it was successfully received
                        if (this.processInParallel)
                        {
                            // Continue receiving and processing new messages asynchronously
                            Task.Factory.StartNew(receiveNext);
                        }
                        // Check if we actually received any messages.
                        if (msg != null)
                        {
                            var roundtripStopwatch = Stopwatch.StartNew();
                            long schedulingElapsedMilliseconds = 0;
                            long processingElapsedMilliseconds = 0;

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
                                    this.ReleaseMessage(msg, releaseAction, processingElapsedMilliseconds, schedulingElapsedMilliseconds, roundtripStopwatch);
                                }

                                if (!this.processInParallel)
                                {
                                    // Continue receiving and processing new messages until told to stop.
                                    receiveNext.Invoke();
                                }
                            });
                        }
                        else
                        {
                            this.dynamicThrottling.NotifyWorkCompleted();
                            if (!this.processInParallel)
                            {
                                // Continue receiving and processing new messages until told to stop.
                                receiveNext.Invoke();
                            }
                        }
                    }
                }));
            });

            // Initialize an action to receive the next message in the queue or end if cancelled.
            receiveNext = () =>
            {
                this.dynamicThrottling.WaitUntilAllowedParallelism(cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    this.dynamicThrottling.NotifyWorkStarted();
                    // Continue receiving and processing new messages until told to stop.
                    receiveMessage.Invoke();
                }
            };

            // Initialize a custom action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            recoverReceive = ex =>
            {
                // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                Trace.TraceError("An unrecoverable error occurred while trying to receive a new message from subscription {1}:\r\n{0}", ex, this.subscription);
                this.dynamicThrottling.NotifyWorkCompletedWithError();

                if (!cancellationToken.IsCancellationRequested)
                {
                    // Continue receiving and processing new messages until told to stop regardless of any exceptions.
                    TaskEx.Delay(10000).ContinueWith(t => receiveMessage.Invoke());
                }
            };

            // Start receiving messages asynchronously.
            receiveNext.Invoke();
        }

        private void ReleaseMessage(BrokeredMessage msg, MessageReleaseAction releaseAction, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            switch (releaseAction.Kind)
            {
                case MessageReleaseActionKind.Complete:
                    msg.SafeCompleteAsync(
                        this.subscription,
                        success =>
                        {
                            msg.Dispose();
                            this.instrumentation.MessageCompleted(success);
                            if (success)
                            {
                                this.dynamicThrottling.NotifyWorkCompleted();
                            }
                            else
                            {
                                this.dynamicThrottling.NotifyWorkCompletedWithError();
                            }
                        },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.Abandon:
                    msg.SafeAbandonAsync(
                        this.subscription,
                        success => { msg.Dispose(); this.instrumentation.MessageCompleted(false); this.dynamicThrottling.NotifyWorkCompletedWithError(); },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.DeadLetter:
                    msg.SafeDeadLetterAsync(
                        this.subscription,
                        releaseAction.DeadLetterReason,
                        releaseAction.DeadLetterDescription,
                        success => { msg.Dispose(); this.instrumentation.MessageCompleted(false); this.dynamicThrottling.NotifyWorkCompletedWithError(); },
                        processingElapsedMilliseconds,
                        schedulingElapsedMilliseconds,
                        roundtripStopwatch);
                    break;
                default:
                    break;
            }
        }

        private void RetryWaring(RetryingEventArgs e)
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
