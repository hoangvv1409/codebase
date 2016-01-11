using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public interface IEventBus
    {
        void Publish(Envelope<IEvent> e);
        void Publish(IEnumerable<Envelope<IEvent>> es);
    }
}
