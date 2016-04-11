using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging.Handling
{
    public class CommandDispatcher
    {
        private Dictionary<Type, ICommandHandler> handlers = new Dictionary<Type, ICommandHandler>();

        public void Register(ICommandHandler commandHandler)
        {
            var genericHandler = typeof(ICommandHandler<>);
            var supportedCommandTypes = commandHandler.GetType()
                .GetInterfaces()
                .Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
                .Select(iface => iface.GetGenericArguments()[0])
                .ToList();
            // moi command chi duoc phep handle boi 1 handler
            if (handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
                throw new ArgumentException("The command handled by the received handler already has a registered handler.");

            // Register this handler for each of he handled types.
            foreach (var commandType in supportedCommandTypes)
            {
                this.handlers.Add(commandType, commandHandler);
            }
        }

        public bool ProcessMessage(ICommand command)
        {
            return this.ProcessMessage(command, null, null, "");
        }

        /// <summary>
        /// Processes the message by calling the registered handler.
        /// </summary>
        public bool ProcessMessage(ICommand payload, string messageId, string correlationId, string traceIdentifier)
        {
            var commandType = payload.GetType();
            ICommandHandler handler = null;

            if (this.handlers.TryGetValue(commandType, out handler))
            {
                // Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Command{0} handled by {1}.", traceIdentifier, handler.GetType().FullName));
                ((dynamic)handler).Handle((dynamic)payload);
                return true;
            }
            // There can be a generic logging/tracing/auditing handlers
            else if (this.handlers.TryGetValue(typeof(ICommand), out handler))
            {
                ((dynamic)handler).Handle((dynamic)payload);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
