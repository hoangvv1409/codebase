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
    public class Order : Aggregate
    {
        public long SOID { get; private set; }
        public long UserId { get; private set; }
        public OrderState OrderState { get; private set; }
        public int? WarehouseId { get; private set; }
        public string WarehouseName { get; private set; }
        public DateTime CreatedDate { get; private set; }

        protected Order()
            : this(Guid.NewGuid())
        { }

        protected Order(Guid id)
            : base(id)
        { }

        public Order(long soid, long userId, OrderState orderState)
            : this()
        {
            this.SOID = soid;
            this.UserId = userId;
            this.OrderState = orderState;
            this.CreatedDate = DateTime.Now;
        }

        public Order(long soid, long userId, OrderState orderState, int warehouseId, string warehouseName)
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
                    return new OrderArrived(this.Id)
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                        WarehouseId = (int)this.WarehouseId,
                        WarehouseName = this.WarehouseName
                    };
                case OrderState.Cancel:
                    return new OrderCancelled(this.Id)
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Confirm:
                    return new OrderConfirmed(this.Id)
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Paid:
                    return new OrderPaided(this.Id)
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.PartiallyCancel:
                    return new OrderPartiallyCancelled(this.Id)
                    {
                        SOID = this.SOID,
                        UserId = this.UserId,
                    };
                case OrderState.Shipping:
                    return new OrderShipBegun(this.Id)
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
