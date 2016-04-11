using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.EventSourcing
{
    /// <summary>
    /// Represents an event message that belongs to an ordered event stream.
    /// </summary>
    public interface IVersionedEvent : IEvent
    {
        /// <summary>
        /// Gets the version or order of the event in the stream.
        /// </summary>
        int Version { get; }
    }
}
