using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Events;

namespace VinEcom.MobileNotification.Tests
{
    [TestClass]
    public class OrderFixture
    {
        [TestMethod]
        public void when_create_order_with_soid_userid_orderstate_then_success()
        {
            Order order = new Order(123456, 123, OrderState.Arrived);

            Assert.IsTrue(123456 == order.SOID);
            Assert.IsTrue(123 == order.UserId);
            Assert.IsTrue(OrderState.Arrived == order.OrderState);
        }

        [TestMethod]
        public void when_create_order_with_soid_userid_orderstate_warehouseid_warehousename_then_success()
        {
            Order order = new Order(123456, 123, OrderState.Arrived, 1, "Test");

            Assert.IsTrue(123456 == order.SOID);
            Assert.IsTrue(123 == order.UserId);
            Assert.IsTrue(OrderState.Arrived == order.OrderState);

            Assert.IsTrue(1 == order.WarehouseId);
            Assert.IsTrue("Test" == order.WarehouseName);
        }

        [TestMethod]
        public void when_orderstate_is_arrived_then_event_is_orderarrived()
        {
            Order order = new Order(123456, 123, OrderState.Arrived, 1, "Test");
            order.GenerateEvent();

            Assert.IsTrue(typeof(OrderArrived) == order.Event.GetType());
        }
    }
}
