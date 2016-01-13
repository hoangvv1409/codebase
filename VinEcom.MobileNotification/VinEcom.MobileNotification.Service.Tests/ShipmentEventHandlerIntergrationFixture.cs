using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Service.Handlers;

namespace VinEcom.MobileNotification.Service.Tests
{
    [TestClass]
    public class ShipmentEventHandlerIntergrationFixture : GivenADbContext
    {
        private ShipmentEventHandler handler;
        private int resourceType = (int)ResourceTypes.Shipment;

        public ShipmentEventHandlerIntergrationFixture()
        {
            IContentGenerator contentGenerator = new ContentGenerator();
            this.handler = new ShipmentEventHandler(() => new MobileNotificationDbContext(dbName), contentGenerator);
        }

        [TestMethod]
        public void when_receive_shipment_arrived_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            ShipmentArrived shipmentArrived = new ShipmentArrived(id)
            {
                SOID = 123456,
                UserId = 123,
                WarehouseId = 1,
                WarehouseName = "Kho tong Ha Noi",
                ShipmentId = 5
            };

            string template =
                "'Gói hàng {ShipmentId} của đơn hàng {SOID} đã được mang đi giao. Xin vui lòng đợi trong giây lát'";
            string content =
                "'Gói hàng 5 của đơn hàng 123456 đã được mang đi giao. Xin vui lòng đợi trong giây lát'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)ShipmentState.Arrived,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(shipmentArrived);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_shipment_ship_begun_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            ShipmentShipBegun shipmentShipBegun = new ShipmentShipBegun(id)
            {
                SOID = 123456,
                UserId = 123,
                ShipmentId = 5
            };

            string template =
                "'Gói hàng {ShipmentID} của đơn hàng {SOID} đã được chuyển tới {WarehouseName} gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính.'";
            string content =
                "'Gói hàng 5 của đơn hàng 123456 đã được chuyển tới Kho tong Ha Noi gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính.'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = resourceType,
                    ResourceState = (int)ShipmentState.Shipping,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(shipmentShipBegun);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }
    }
}
