using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public interface IMessageReceiver
    {
        void Start(Func<BrokeredMessage, MessageReleaseAction> messageHandler);
        void Stop();
    }
}
