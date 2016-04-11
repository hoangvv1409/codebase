using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging
{
    /// <summary>
    /// An event bus that sends serialized object payloads.
    /// </summary>
    /// <remarks>Note that <see cref="Infrastructure.EventSourcing.IEventSourced"/> entities persisted through 
    /// the <see cref="Infrastructure.EventSourcing.IEventSourcedRepository{T}"/> do not
    /// use the <see cref="IEventBus"/>, but has its own event publishing mechanism.</remarks>
    public interface IEventBus
    {
        void Publish(Envelope<IEvent> @event);
        void Publish(IEnumerable<Envelope<IEvent>> events);
    }
}
