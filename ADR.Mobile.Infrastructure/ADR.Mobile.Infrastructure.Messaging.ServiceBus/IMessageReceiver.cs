using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus
{
    /// <summary>
    /// Abstracts the behavior of a receiving component that raises 
    /// an event for every received event.
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// Starts the listener.
        /// </summary>
        /// <param name="messageHandler">Handler for incoming messages. The return value indicates how to release the message lock.</param>
        void Start(Func<BrokeredMessage, MessageReleaseAction> messageHandler);

        /// <summary>
        /// Stops the listener.
        /// </summary>
        void Stop();
    }
}
