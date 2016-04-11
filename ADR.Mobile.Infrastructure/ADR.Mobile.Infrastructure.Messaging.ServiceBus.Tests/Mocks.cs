
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus;

namespace Infrastructure.Messaging.ServiceBus.Tests
{
    class MessageSenderMock : IMessageSender
    {
        public readonly AutoResetEvent SendSignal = new AutoResetEvent(false);
        public readonly ConcurrentBag<BrokeredMessage> Sent = new ConcurrentBag<BrokeredMessage>();
        public readonly ConcurrentBag<Action> AsyncSuccessCallbacks = new ConcurrentBag<Action>();

        public bool ShouldWaitForCallback { get; set; }

        void IMessageSender.Send(Func<BrokeredMessage> messageFactory)
        {
            this.Sent.Add(messageFactory.Invoke());
            this.SendSignal.Set();
        }

        void IMessageSender.SendAsync(Func<BrokeredMessage> messageFactory)
        {
            throw new NotImplementedException();
        }

        void IMessageSender.SendAsync(Func<BrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback)
        {
            Task.Factory.StartNew(
                () =>
                {
                    this.Sent.Add(messageFactory.Invoke());
                    this.SendSignal.Set();
                    if (!this.ShouldWaitForCallback)
                    {
                        successCallback();
                    }
                    else
                    {
                        AsyncSuccessCallbacks.Add(successCallback);
                    }
                },
                TaskCreationOptions.AttachedToParent);
        }

        public event EventHandler Retrying;
    }
}
