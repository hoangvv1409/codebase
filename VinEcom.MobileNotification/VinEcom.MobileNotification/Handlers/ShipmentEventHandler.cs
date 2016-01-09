using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Handlers
{
    public class ShipmentEventHandler :
        IEventHandler<ShipmentArrived>,
        IEventHandler<ShipmentShipBegun>
    {
        public void Handle(ShipmentArrived @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(ShipmentShipBegun @event)
        {
            throw new NotImplementedException();
        }
    }
}
