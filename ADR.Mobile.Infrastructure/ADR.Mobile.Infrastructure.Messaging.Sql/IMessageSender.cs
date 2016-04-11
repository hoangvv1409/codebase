using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging.Sql
{
    /// <summary>
    /// Abstracts the behavior of sending a message.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the specified message.
        /// </summary>
        void Send(Message message);

        /// <summary>
        /// Sends a batch of messages.
        /// </summary>
        void Send(IEnumerable<Message> messages);
    }
}
