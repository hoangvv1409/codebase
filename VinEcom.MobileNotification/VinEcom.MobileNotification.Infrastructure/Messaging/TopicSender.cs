﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public class TopicSender : IMessageSender
    {
        private readonly ServiceBusSettings settings;
        private readonly string topic;
        private readonly RetryPolicy retryPolicy;
        private readonly RetryStrategy retryStrategy;
        private readonly int maxNumberRetry;
        private readonly TopicClient topicClient;
        //private readonly MsmqSender msmqSender;
        //private readonly MessageSenderPool senderPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicSender"/> class, 
        /// automatically creating the given topic if it does not exist.
        /// </summary>
        public TopicSender(ServiceBusSettings settings, string topic)
            : this(settings, topic, new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1)))
        {
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="TopicSender"/> class, 
        ///// automatically creating the given topic if it does not exist.
        ///// </summary>
        protected TopicSender(ServiceBusSettings settings, string topic, RetryStrategy retryStrategy)
        {
            this.settings = settings;
            this.topic = topic;

            this.retryStrategy = retryStrategy;
            var factory = MessagingFactory.CreateFromConnectionString(this.settings.ConnectionString);
            this.topicClient = factory.CreateTopicClient(this.topic);
            //this.senderPool = new MessageSenderPool(this.settings.ConnectionString, topic);

            this.retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(this.retryStrategy);
            this.retryPolicy.Retrying += (s, e) =>
            {
                var handler = this.Retrying;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                Trace.TraceWarning("An error occurred in attempt number {1} to send a message: {0}", e.LastException.Message, e.CurrentRetryCount);
            };
        }

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        public event EventHandler Retrying;

        /// <summary>
        /// Asynchronously sends the specified message.
        /// </summary>
        public void SendAsync(Func<BrokeredMessage> messageFactory)
        {
            // TODO: SendAsync is not currently being used by the app or VinEcom.Transportation.Infrastructure.
            // Consider removing or have a callback notifying the result.
            // Always send async.
            this.SendAsync(messageFactory, () => { }, ex => { });
        }

        public void SendAsync(IEnumerable<Func<BrokeredMessage>> messageFactories)
        {
            // TODO: batch/transactional sending?
            foreach (var messageFactory in messageFactories)
            {
                this.SendAsync(messageFactory);
            }
        }

        public void Send(Func<BrokeredMessage> messageFactory)
        {
            var resetEvent = new ManualResetEvent(false);
            Exception exception = null;

            this.SendAsync(
                messageFactory,
                () =>
                {
                    //resetEvent.Set();
                },
                ex =>
                {
                    exception = ex;
                    //resetEvent.Set();
                });

            //resetEvent.WaitOne();
            if (exception != null)
            {
                throw exception;
            }
        }

        protected virtual void DoBeginSendMessage(BrokeredMessage message, AsyncCallback ac)
        {
            try
            {
                this.topicClient.BeginSend(message, ac, message);
            }
            catch
            {
                message.Dispose();
                throw;
            }
        }

        protected virtual void DoEndSendMessage(IAsyncResult ar)
        {
            try
            {
                this.topicClient.EndSend(ar);
            }
            finally
            {
                using (ar.AsyncState as IDisposable) { }
            }
        }


        public void SendAsync(Func<BrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback)
        {
            throw new NotImplementedException();
        }
    }
}