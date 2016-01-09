using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public class OrderNotification : Aggregate
    {
        public long SOID { get; private set; }
        public int UserId { get; private set; }
        public OrderState OrderState { get; private set; }
        public int WarehouseId { get; private set; }
        public string WarehouseName { get; private set; }

        public OrderNotification(long soid, int userId, OrderState orderState)
        {
            this.SOID = soid;
            this.UserId = userId;
            this.OrderState = orderState;
        }

        public OrderNotification(long soid, int userId, OrderState orderState, int warehouseId, string warehouseName)
            : this(soid, userId, orderState)
        {
            this.WarehouseId = warehouseId;
            this.WarehouseName = warehouseName;
        }

        protected override IEvent EventFactory()
        {
            switch (this.OrderState)
            {
                case OrderState.Arrived:
                    return new OrderArrived()
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                        WarehouseId = this.WarehouseId,
                        WarehouseName = this.WarehouseName
                    };
                case OrderState.Cancel:
                    return new OrderCancelled()
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Confirm:
                    return new OrderConfirmed
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Paid:
                    return new OrderPaided
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.PartiallyCancel:
                    return new OrderPartiallyCancelled()
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Shipping:
                    return new OrderShipBegun()
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                default:
                    return null;
            }
        }
    }
}
