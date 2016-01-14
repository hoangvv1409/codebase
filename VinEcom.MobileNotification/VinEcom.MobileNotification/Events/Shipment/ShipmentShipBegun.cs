using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Events
{
    public class ShipmentShipBegun : IEvent
    {
        public ShipmentShipBegun(Guid id)
        {
            this.SourceId = id;
        }

        public Guid SourceId { get; private set; }
        public int ShipmentId { get; set; }
        public long SOID { get; set; }
        public long UserId { get; set; }

        public ShipmentState ShipmentState
        {
            get { return ShipmentState.Shipping; }
        }
    }
}
