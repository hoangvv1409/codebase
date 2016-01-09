using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public interface IMessageSender
    {
        void Send(Func<BrokeredMessage> messageFactory);
        void SendAsync(Func<BrokeredMessage> messageFactory);
        void SendAsync(Func<BrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback);
    }
}
