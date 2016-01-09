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
    public class ShipmentNotification : Aggregate
    {
        public int ShipmentId { get; private set; }
        public long SOID { get; private set; }
        public ShipmentState ShipmentState { get; private set; }
        public int UserId { get; private set; }
        public string WarehouseName { get; private set; }
        public int WarehouseId { get; private set; }

        public ShipmentNotification(int shipmentId, long soid, int userId, ShipmentState shipmentState)
        {
            this.ShipmentId = shipmentId;
            this.SOID = soid;
            this.UserId = userId;
            this.ShipmentState = shipmentState;
        }

        public ShipmentNotification(int shipmentId, long soid, int userId, ShipmentState shipmentState, string warehouseName, int warehouseId)
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
                    return new ShipmentArrived()
                    {
                        ShipmentId = this.ShipmentId,
                        SOID = this.SOID,
                        UserId = this.UserId,
                        WarehouseName = this.WarehouseName,
                        WarehouseId = this.WarehouseId
                    };
                case ShipmentState.Shipping:
                    return new ShipmentShipBegun()
                    {
                        ShipmentId = this.ShipmentId,
                        SOID = this.SOID,
                        UserId = this.UserId,
                        WarehouseName = this.WarehouseName,
                        WarehouseId = this.WarehouseId
                    };
                default:
                    return null;
            }
        }
    }
}
