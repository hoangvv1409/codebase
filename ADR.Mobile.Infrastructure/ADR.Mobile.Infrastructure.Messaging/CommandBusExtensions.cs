using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging
{
    /// <summary>
    /// Provides usability overloads for <see cref="ICommandBus"/>
    /// </summary>
    public static class CommandBusExtensions
    {
        public static void Send(this ICommandBus bus, ICommand command)
        {
            bus.Send(new Envelope<ICommand>(command));
        }

        public static void Send(this ICommandBus bus, IEnumerable<ICommand> commands)
        {
            bus.Send(commands.Select(x => new Envelope<ICommand>(x)));
        }
    }
}
