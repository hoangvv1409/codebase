using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Handlers
{
    public class OrderEventHandler :
        IEventHandler<OrderArrived>,
        IEventHandler<OrderCancelled>,
        IEventHandler<OrderConfirmed>,
        IEventHandler<OrderPaided>,
        IEventHandler<OrderPartiallyCancelled>,
        IEventHandler<OrderShipBegun>
    {
        public void Handle(OrderArrived @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(OrderCancelled @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(OrderConfirmed @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(OrderPaided @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(OrderPartiallyCancelled @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(OrderShipBegun @event)
        {
            throw new NotImplementedException();
        }
    }
}
