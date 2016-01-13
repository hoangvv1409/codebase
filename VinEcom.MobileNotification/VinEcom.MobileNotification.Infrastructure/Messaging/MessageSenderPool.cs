using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public class MessageSenderPool : IMessageSenderPool
    {
        protected MessagingFactory[] messagingFactories;
        protected int numberSender = 5;
        protected MessageSender[] senders;
        protected string connectionString;
        protected string topic;
        private Random random = new Random();
        public MessageSenderPool(string serviceBusConnectionString, string topic)
        {
            this.connectionString = serviceBusConnectionString;
            this.topic = topic;
            this.CreateEntity();
        }

        protected virtual void CreateEntity()
        {
            this.messagingFactories = new MessagingFactory[this.numberSender];
            this.senders = new MessageSender[this.numberSender];

            // Create senders.
            int factoryIndex = 0;
            for (int i = 0; i < this.numberSender; i++)
            {
                messagingFactories[factoryIndex] = MessagingFactory.CreateFromConnectionString(connectionString);
                senders[i] = messagingFactories[factoryIndex++].CreateMessageSender(this.topic);
            }
        }

        public virtual MessageSender GetMessageSender()
        {
            int index = this.random.Next(0, numberSender);
            return this.senders[index];
        }
    }
}
