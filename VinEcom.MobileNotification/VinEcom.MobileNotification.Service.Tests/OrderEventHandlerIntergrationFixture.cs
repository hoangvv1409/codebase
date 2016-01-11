using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Service.Handlers;

namespace VinEcom.MobileNotification.Service.Tests
{
    [TestClass]
    public class OrderEventHandlerIntergrationFixture : GivenADbContext
    {
        private OrderEventHandler handler;
        private int resourceType = (int)ResourceTypes.Order;

        public OrderEventHandlerIntergrationFixture()
        {
            IContentGenerator contentGenerator = new ContentGenerator();
            this.handler = new OrderEventHandler(() => new MobileNotificationDbContext(dbName), contentGenerator);
        }

        [TestMethod]
        public void when_receive_order_arrived_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderArrived orderArrived = new OrderArrived(id)
            {
                SOID = 123456,
                UserId = 123,
                WarehouseId = 1,
                WarehouseName = "Kho tong Ha Noi"
            };

            string template =
                "Đơn hàng {SOID} đã được chuyển tới {WarehouseName} gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính";
            string content =
                "Đơn hàng 123456 đã được chuyển tới Kho tong Ha Noi gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.Arrived,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderArrived);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_order_cancelled_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderCancelled orderCancelled = new OrderCancelled(id)
            {
                SOID = 123456,
                UserId = 123
            };

            string template =
                "'Đơn hàng {SOID} của Quý khách đã được hủy theo yêu cầu'";
            string content =
                "'Đơn hàng 123456 của Quý khách đã được hủy theo yêu cầu'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.Cancel,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderCancelled);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_order_confirmed_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderConfirmed orderConfirmed = new OrderConfirmed(id)
            {
                SOID = 123456,
                UserId = 123
            };

            string template =
                "'Đơn hàng {SOID} của Quý khách đã được xác nhận. Adayroi.com sẽ nhanh chóng xử lý đơn hàng này. Cám ơn Quý khách đã sử dụng dịch vụ của Adayroi.com'";
            string content =
                "'Đơn hàng 123456 của Quý khách đã được xác nhận. Adayroi.com sẽ nhanh chóng xử lý đơn hàng này. Cám ơn Quý khách đã sử dụng dịch vụ của Adayroi.com'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.Confirm,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderConfirmed);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_order_paided_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderPaided orderPaided = new OrderPaided(id)
            {
                SOID = 123456,
                UserId = 123
            };

            string template =
                "'Đơn hàng {SOID} của Quý khách đã được thanh toán thành công'";
            string content =
                "'Đơn hàng 123456 của Quý khách đã được thanh toán thành công'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.Paid,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderPaided);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_order_partially_cancelled_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderPartiallyCancelled orderPaided = new OrderPartiallyCancelled(id)
            {
                SOID = 123456,
                UserId = 123
            };

            string template =
                "'Đơn hàng {SOID} đã được thay đổi theo yêu cầu của Quý khách. Vui lòng nhấn vào đây để xem chi tiết'";
            string content =
                "'Đơn hàng 123456 đã được thay đổi theo yêu cầu của Quý khách. Vui lòng nhấn vào đây để xem chi tiết'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.PartiallyCancel,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderPaided);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_order_ship_begun_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            OrderShipBegun orderShipBegun = new OrderShipBegun(id)
            {
                SOID = 123456,
                UserId = 123
            };

            string template =
                "'Đơn hàng {SOID} đang được vận chuyển đến địa chỉ nhận hàng của Quý khách'";
            string content =
                "'Đơn hàng 123456 đang được vận chuyển đến địa chỉ nhận hàng của Quý khách'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)OrderState.Shipping,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(orderShipBegun);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }
    }
}
