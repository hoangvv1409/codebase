using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Serialization;

namespace ADR.Mobile.Infrastructure.Messaging.Sql
{
    /// <summary>
    /// This is an extremely basic implementation of <see cref="ICommandBus"/> that is used only for running the sample
    /// application without the dependency to the Windows Azure Service Bus when using the DebugLocal solution configuration.
    /// It should not be used in production systems.
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IMessageSender sender;
        private readonly ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus"/> class.
        /// </summary>
        /// <param name="serializer">The serializer to use for the message body.</param>
        public CommandBus(IMessageSender sender, ITextSerializer serializer)
        {
            this.sender = sender;
            this.serializer = serializer;
        }

        /// <summary>
        /// Sends the specified command.
        /// </summary>
        public void Send(Envelope<ICommand> command)
        {
            var message = BuildMessage(command);

            this.sender.Send(message);
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            var messages = commands.Select(command => BuildMessage(command));

            this.sender.Send(messages);
        }

        private Message BuildMessage(Envelope<ICommand> command)
        {
            // TODO: should use the Command ID as a unique constraint when storing it.
            using (var payloadWriter = new StringWriter())
            {
                this.serializer.Serialize(payloadWriter, command.Body);
                return new Message(payloadWriter.ToString(), command.Delay != TimeSpan.Zero ? (DateTime?)DateTime.UtcNow.Add(command.Delay) : null, command.CorrelationId);
            }
        }
    }
}
