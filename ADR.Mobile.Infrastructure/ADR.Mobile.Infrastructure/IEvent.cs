using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure
{
    /// <summary>
    /// Represents an event message.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the identifier of the source originating the event.
        /// </summary>
        Guid SourceId { get; }
    }
}
