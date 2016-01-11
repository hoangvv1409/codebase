using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Events
{
    public class ShipmentArrived : IEvent
    {
        public ShipmentArrived(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; private set; }
        public int ShipmentId { get; set; }
        public long SOID { get; set; }
        public long UserId { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        public ShipmentState ShipmentState
        {
            get { return ShipmentState.Arrived; }
        }
    }
}
