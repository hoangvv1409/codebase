using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus
{
    /// <summary>
    /// Abstracts the behavior of sending a message.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the specified message synchronously.
        /// </summary>
        void Send(Func<BrokeredMessage> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<BrokeredMessage> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<BrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback);

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        event EventHandler Retrying;
    }
}
