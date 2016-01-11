using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging.Handling
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<T> : IEventHandler
        where T : IEvent
    {
        void Handle(T e);
    }

    public interface IEnvelopedEventHandler<T> : IEventHandler
        where T : IEvent
    {
        void Handle(Envelope<T> envelope);
    }
}
