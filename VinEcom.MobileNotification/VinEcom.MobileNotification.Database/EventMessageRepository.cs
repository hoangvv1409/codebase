using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Database
{
    public class EventMessageRepository<T> : Repository<T> where T : Aggregate
    {
        private readonly ITextSerializer serializer;
        private readonly IEventBus eventBus;

        public EventMessageRepository(Func<MobileNotificationDbContext> dbContextFactory)
            : base(dbContextFactory)
        { }

        public override void Save(T aggregate)
        {
            var @event = aggregate.Event;
            using (var context = dbContext)
            {
                var eventsSet = context.Set<EventMessage>();
                eventsSet.Add(this.Serialize(@event));

                context.SaveChanges();
            }

            this.eventBus.Publish(new Envelope<IEvent>(@event));
        }

        private EventMessage Serialize(IEvent e, string correlationId = "")
        {
            EventMessage serialized;
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(writer, e);
                serialized = new EventMessage
                {
                    Payload = writer.ToString(),
                    CorrelationId = correlationId
                };
            }
            return serialized;
        }
    }
}
