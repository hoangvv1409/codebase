using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public class Shipment : Aggregate
    {
        public int ShipmentId { get; private set; }
        public long SOID { get; private set; }
        public ShipmentState ShipmentState { get; private set; }
        public long UserId { get; private set; }
        public string WarehouseName { get; private set; }
        public int WarehouseId { get; private set; }
        public DateTime CreatedDate { get; private set; }

        protected Shipment()
            : this(Guid.NewGuid())
        { }

        protected Shipment(Guid id)
            : base(id)
        { }

        public Shipment(int shipmentId, long soid, long userId, ShipmentState shipmentState)
            : this()
        {
            this.ShipmentId = shipmentId;
            this.SOID = soid;
            this.UserId = userId;
            this.ShipmentState = shipmentState;
            this.CreatedDate = DateTime.Now;
        }

        public Shipment(int shipmentId, long soid, long userId, ShipmentState shipmentState, int warehouseId, string warehouseName)
            : this(shipmentId, soid, userId, shipmentState)
        {
            this.WarehouseId = warehouseId;
            this.WarehouseName = warehouseName;
        }

        protected override IEvent EventFactory()
        {
            switch (this.ShipmentState)
            {
                case ShipmentState.Arrived:
                    return new ShipmentArrived(this.Id)
                    {
                        ShipmentId = this.ShipmentId,
                        SOID = this.SOID,
                        UserId = this.UserId,
                        WarehouseName = this.WarehouseName,
                        WarehouseId = this.WarehouseId
                    };
                case ShipmentState.Shipping:
                    return new ShipmentShipBegun(this.Id)
                    {
                        ShipmentId = this.ShipmentId,
                        SOID = this.SOID,
                        UserId = this.UserId
                    };
                default:
                    return null;
            }
        }
    }
}
