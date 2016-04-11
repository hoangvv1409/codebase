using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using ADR.Mobile.Infrastructure.Serialization;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus.Handling
{
    public class CommandProcessor : MessageProcessor, ICommandHandlerRegistry
    {
        private CommandDispatcher commandDispatcher;

        public CommandProcessor(IMessageReceiver receiver, ITextSerializer textSerializer)
            : base(receiver, textSerializer)
        {
            this.commandDispatcher = new CommandDispatcher();
        }

        public void Register(ICommandHandler handler)
        {
            this.commandDispatcher.Register(handler);
        }

        protected override void ProcessMessage(string traceIdentifier, object payload, string messageId, string correlationId)
        {
            this.commandDispatcher.ProcessMessage((ICommand)payload, messageId, correlationId, traceIdentifier);
        }
    }
}
