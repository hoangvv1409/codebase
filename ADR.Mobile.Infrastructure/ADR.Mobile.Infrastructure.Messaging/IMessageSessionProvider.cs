using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging
{
    /// <summary>
    /// If a command message implements <see cref="IMessageSessionProvider"/>, it hints implementations of 
    /// <see cref="ICommandBus"/> to assign the specified SessionId to the outgoing messages if supported.
    /// </summary>
    public interface IMessageSessionProvider
    {
        string SessionId { get; }
    }
}
