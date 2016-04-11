using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Serialization;

namespace ADR.Mobile.Infrastructure.Messaging.Sql
{
    /// <summary>
    /// This is an extremely basic implementation of <see cref="IEventBus"/> that is used only for running the sample
    /// application without the dependency to the Windows Azure Service Bus when using the DebugLocal solution configuration.
    /// It should not be used in production systems.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IMessageSender sender;
        private readonly ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <param name="serializer">The serializer to use for the message body.</param>
        public EventBus(IMessageSender sender, ITextSerializer serializer)
        {
            this.sender = sender;
            this.serializer = serializer;
        }

        /// <summary>
        /// Sends the specified event.
        /// </summary>
        public void Publish(Envelope<IEvent> @event)
        {
            var message = this.BuildMessage(@event);

            this.sender.Send(message);
        }

        /// <summary>
        /// Publishes the specified events.
        /// </summary>
        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            var messages = events.Select(e => this.BuildMessage(e));

            this.sender.Send(messages);
        }

        private Message BuildMessage(Envelope<IEvent> @event)
        {
            using (var payloadWriter = new StringWriter())
            {
                this.serializer.Serialize(payloadWriter, @event.Body);
                return new Message(payloadWriter.ToString(), correlationId: @event.CorrelationId);
            }
        }
    }
}
