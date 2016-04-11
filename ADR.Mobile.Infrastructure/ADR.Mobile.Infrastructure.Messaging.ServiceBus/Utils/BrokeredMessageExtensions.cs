using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus.Utils
{
    public static class BrokeredMessageExtensions
    {
        private static readonly RetryStrategy retryStrategy =
            new ExponentialBackoff(3, TimeSpan.FromSeconds(.5d), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)) { FastFirstRetry = true };

        public static void SafeCompleteAsync(this BrokeredMessage message, string subscription, Action<bool> callback, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            SafeMessagingActionAsync(
                message.CompleteAsync(),
                message,
                callback,
                "An error occurred while completing message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                processingElapsedMilliseconds,
                schedulingElapsedMilliseconds,
                roundtripStopwatch);
        }

        public static void SafeAbandonAsync(this BrokeredMessage message, string subscription, Action<bool> callback, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            SafeMessagingActionAsync(
                message.AbandonAsync(),
                message,
                callback,
                "An error occurred while abandoning message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                processingElapsedMilliseconds,
                schedulingElapsedMilliseconds,
                roundtripStopwatch);
        }

        public static void SafeDeadLetterAsync(this BrokeredMessage message, string subscription, string reason, string description, Action<bool> callback, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            SafeMessagingActionAsync(
                message.DeadLetterAsync(reason, description),
                message,
                callback,
                "An error occurred while dead-lettering message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                processingElapsedMilliseconds,
                schedulingElapsedMilliseconds,
                roundtripStopwatch);
        }

        internal static void SafeMessagingActionAsync(Task messageAction, BrokeredMessage message, Action<bool> callback, string actionErrorDescription, string messageId, string subscription, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying +=
                (s, e) =>
                {
                    Trace.TraceWarning("An error occurred in attempt number {1} to release message {3} in subscription {2}: {0}",
                    e.LastException.GetType().Name + " - " + e.LastException.Message,
                    e.CurrentRetryCount,
                    subscription,
                    message.MessageId);
                };

            long messagingActionStart = roundtripStopwatch.ElapsedMilliseconds;

            retryPolicy.ExecuteAsync(() => messageAction.ContinueWith(r =>
            {
                if (r.Exception != null)
                {
                    Exception e = r.Exception;
                    roundtripStopwatch.Stop();
                    if (e is MessageLockLostException || e is MessagingException || e is TimeoutException)
                    {
                        Trace.TraceWarning(actionErrorDescription, messageId, subscription, e.GetType().Name + " - " + e.Message, processingElapsedMilliseconds, schedulingElapsedMilliseconds, messagingActionStart, roundtripStopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        Trace.TraceError("Unexpected error releasing message in subscription {1}:\r\n{0}", e, subscription);
                    }

                    callback(false);
                }
                else
                {
                    messagingActionStart = roundtripStopwatch.ElapsedMilliseconds;
                    roundtripStopwatch.Stop();
                    callback(true);
                }
            }));
        }
    }
}
