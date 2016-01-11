using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.Database.Test;
using VinEcom.MobileNotification.Enums;

namespace VinEcom.MobileNotification.Database.Tests
{
    [TestClass]
    public class OrderRepositoryFixture : GivenADbContext
    {
        private Func<MobileNotificationDbContext> dbContext;

        public OrderRepositoryFixture()
        {
            this.dbContext = () => new MobileNotificationDbContext(dbName);
        }

        [TestMethod]
        public void when_save_order_then_insert_success()
        {
            Order orderNotification = new Order(123456, 123, OrderState.Arrived);
            OrderRepository repository = new OrderRepository(dbContext);
            repository.Save(orderNotification);
            repository.SaveChanges();

            var order = repository.Find(orderNotification.Id);

            Assert.IsTrue(order.Id == orderNotification.Id);
            Assert.IsTrue(order.CreatedDate == orderNotification.CreatedDate);
            Assert.IsTrue(order.OrderState == orderNotification.OrderState);
            Assert.IsTrue(order.UserId == orderNotification.UserId);
        }

        [TestMethod]
        public void when_save_exist_order_then_insert_fail()
        {
            Order orderNotification = new Order(123456, 123, OrderState.Arrived);
            OrderRepository repository = new OrderRepository(dbContext);
            repository.Save(orderNotification);
            repository.SaveChanges();

            try
            {
                repository = new OrderRepository(dbContext);
                repository.Save(orderNotification);
                repository.SaveChanges();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message != string.Empty);
            }
        }
    }
}
