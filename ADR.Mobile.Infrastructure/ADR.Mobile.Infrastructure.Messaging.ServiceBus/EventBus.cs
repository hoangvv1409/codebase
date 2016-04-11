using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Serialization;
using Microsoft.ServiceBus.Messaging;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus
{
    /// <summary>
    /// An event bus that sends serialized object payloads through a <see cref="IMessageSender"/>.
    /// </summary>
    /// <remarks>Note that <see cref="Infrastructure.EventSourcing.IEventSourced"/> entities persisted through the <see cref="IEventSourcedRepository{T}"/>
    /// do not use the <see cref="IEventBus"/>, but has its own event publishing mechanism.</remarks>
    public class EventBus : IEventBus
    {
        private readonly IMessageSender sender;
        private readonly IMetadataProvider metadataProvider;
        private readonly ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <param name="serializer">The serializer to use for the message body.</param>
        public EventBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer)
        {
            this.sender = sender;
            this.metadataProvider = metadataProvider;
            this.serializer = serializer;
        }

        /// <summary>
        /// Sends the specified event.
        /// </summary>
        public void Publish(Envelope<IEvent> @event)
        {
            this.sender.Send(() => BuildMessage(@event));
        }

        /// <summary>
        /// Publishes the specified events.
        /// </summary>
        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            foreach (var @event in events)
            {
                this.Publish(@event);
            }
        }

        private BrokeredMessage BuildMessage(Envelope<IEvent> envelope)
        {
            var @event = envelope.Body;

            var stream = new MemoryStream();
            try
            {
                var writer = new StreamWriter(stream);
                this.serializer.Serialize(writer, @event);
                stream.Position = 0;

                var message = new BrokeredMessage(stream, true);

                message.SessionId = @event.SourceId.ToString();

                if (!string.IsNullOrWhiteSpace(envelope.MessageId))
                {
                    message.MessageId = envelope.MessageId;
                }

                if (!string.IsNullOrWhiteSpace(envelope.CorrelationId))
                {
                    message.CorrelationId = envelope.CorrelationId;
                }

                var metadata = this.metadataProvider.GetMetadata(@event);
                if (metadata != null)
                {
                    foreach (var pair in metadata)
                    {
                        message.Properties[pair.Key] = pair.Value;
                    }
                }

                return message;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }
    }
}
