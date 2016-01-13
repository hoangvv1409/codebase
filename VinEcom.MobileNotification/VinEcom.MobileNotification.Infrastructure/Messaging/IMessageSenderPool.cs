using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public interface IMessageSenderPool
    {
        MessageSender GetMessageSender();
    }
}
