using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public class EventBus : IEventBus
    {
        private IMessageSender messageSender;
        private ITextSerializer serializer;
        private IMetadataProvider metadataProvider;

        public EventBus(IMessageSender sender, ITextSerializer serializer, IMetadataProvider metadataProvider)
        {
            this.messageSender = sender;
            this.serializer = serializer;
            this.metadataProvider = metadataProvider;
        }

        public void Publish(Envelope<IEvent> e)
        {
            this.messageSender.Send(() => BuildMessage(e));
        }

        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            foreach (var e in events)
            {
                this.messageSender.Send(() => BuildMessage(e));
            }
        }

        private BrokeredMessage BuildMessage(Envelope<IEvent> envelope)
        {
            var e = envelope.Body;

            var stream = new MemoryStream();
            try
            {
                var writer = new StreamWriter(stream);
                this.serializer.Serialize(writer, e);
                stream.Position = 0;

                var message = new BrokeredMessage(stream, true);

                //message.SessionId = e.SourceId.ToString();

                if (!string.IsNullOrWhiteSpace(envelope.MessageId))
                {
                    message.MessageId = envelope.MessageId;
                }

                if (!string.IsNullOrWhiteSpace(envelope.CorrelationId))
                {
                    message.CorrelationId = envelope.CorrelationId;
                }

                var metadata = this.metadataProvider.GetMetadata(e);
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
