using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging
{
    /// <summary>
    /// Provides usability overloads for <see cref="IEventBus"/>
    /// </summary>
    public static class EventBusExtensions
    {
        public static void Publish(this IEventBus bus, IEvent @event)
        {
            bus.Publish(new Envelope<IEvent>(@event));
        }

        public static void Publish(this IEventBus bus, IEnumerable<IEvent> events)
        {
            bus.Publish(events.Select(x => new Envelope<IEvent>(x)));
        }
    }
}
