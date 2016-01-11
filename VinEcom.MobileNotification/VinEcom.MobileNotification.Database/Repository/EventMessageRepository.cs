using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Database.Repository
{
    public class EventMessageRepository : Repository<EventMessage>
    {
        private IEventBus bus;
        private ITextSerializer serializer;

        public EventMessageRepository(Func<MobileNotificationDbContext> dbContextFactory, IEventBus bus, ITextSerializer serializer)
            : base(dbContextFactory)
        {
            this.bus = bus;
            this.serializer = serializer;
        }

        public void Save(Aggregate aggregate)
        {
            IEvent e = aggregate.Event;
            EventMessage eventMessage = this.Serialize(e);
            base.Save(eventMessage);
            base.SaveChanges();

            this.bus.Publish(new Envelope<IEvent>(e));
        }

        private EventMessage Serialize(IEvent e)
        {
            EventMessage serialized;
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(writer, e);
                serialized = new EventMessage
                {
                    Id = e.Id,
                    Payload = writer.ToString(),
                };
            }
            return serialized;
        }
    }
}
