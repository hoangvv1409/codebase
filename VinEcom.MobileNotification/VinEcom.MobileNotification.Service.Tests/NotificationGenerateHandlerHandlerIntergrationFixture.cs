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
    public class NotificationGenerateHandlerHandlerIntergrationFixture : GivenADbContext
    {
        private NotificationGenerateHandler handler;

        public NotificationGenerateHandlerHandlerIntergrationFixture()
        {
            IContentGenerator contentGenerator = new ContentGenerator();
            this.handler = new NotificationGenerateHandler(() => new MobileNotificationDbContext(dbName), contentGenerator);
        }

        #region Order
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
                    ResourceType = (int)ResourceTypes.Order,
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
                    ResourceType = (int)ResourceTypes.Order,
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
                    ResourceType = (int)ResourceTypes.Order,
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
                    ResourceType = (int)ResourceTypes.Order,
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
                    ResourceType = (int)ResourceTypes.Order,
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
                    ResourceType = (int)ResourceTypes.Order,
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
        #endregion

        #region Shipment
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

            string template = "'Gói hàng {ShipmentID} của đơn hàng {SOID} đã được chuyển tới {WarehouseName} gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính.'";
            string content = "'Gói hàng 5 của đơn hàng 123456 đã được chuyển tới Kho tong Ha Noi gần nhất. Vui lòng đến nhận bất kỳ lúc nào trong giờ hành chính.'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.Shipment,
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
                ShipmentId = 5,
            };

            string template =
               "'Gói hàng {ShipmentId} của đơn hàng {SOID} đã được mang đi giao. Xin vui lòng đợi trong giây lát'";
            string content =
                "'Gói hàng 5 của đơn hàng 123456 đã được mang đi giao. Xin vui lòng đợi trong giây lát'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.Shipment,
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
        #endregion

        #region User
        [TestMethod]
        public void when_receive_adr_points_used_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            AdrPointsUsed adrPointsUsed = new AdrPointsUsed(id)
            {
                UserId = 123,
                AdrPoints = 50000,
                SOID = 123456
            };

            string template =
                "'Tài khoản điểm Adayroi của Quý khách vừa sử dụng {AdrPoints} để thanh toán đơn hàng {SOID}'";
            string content =
                "'Tài khoản điểm Adayroi của Quý khách vừa sử dụng 50000 để thanh toán đơn hàng 123456'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.AdrPointsUsed,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(adrPointsUsed);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_adr_points_added_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            AdrPointAdded adrPointAdded = new AdrPointAdded(id)
            {
                UserId = 123,
                AdrPoints = 50000,
            };

            string template =
                "'Tài khoản điểm Adayroi của Quý khách vừa được cộng thêm {AdrPoints} điểm.'";
            string content =
                "'Tài khoản điểm Adayroi của Quý khách vừa được cộng thêm 50000 điểm.'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.AdrPointAdded,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(adrPointAdded);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_adr_points_refunded_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            AdrPointsRefunded adrPointsRefunded = new AdrPointsRefunded(id)
            {
                UserId = 123,
                AdrPoints = 50000,
                SOID = 123456
            };

            string template =
                "'Tài khoản điểm Adayroi của Quý khách được hoàn {AdrPoints} điểm do đơn hàng {SOID} đã được hủy trước đó'";
            string content =
                "'Tài khoản điểm Adayroi của Quý khách được hoàn 50000 điểm do đơn hàng 123456 đã được hủy trước đó'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.AdrPointsRefunded,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(adrPointsRefunded);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_password_changed_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            PasswordChanged passwordChanged = new PasswordChanged(id)
            {
                UserId = 123,
            };

            string template =
                "'Mật khẩu của quý khách đã được thay đổi. Xin vui lòng đăng nhập lại'";
            string content =
                "'Mật khẩu của quý khách đã được thay đổi. Xin vui lòng đăng nhập lại'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.PasswordChanged,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(passwordChanged);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_two_step_activated_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            TwoStepsActivated twoStepsActivated = new TwoStepsActivated(id)
            {
                UserId = 123,
            };

            string template =
                "'Chức năng bảo vệ 2 bước đã được kích hoạt thành công cho tài khoản của Quý khách.'";
            string content =
                "'Chức năng bảo vệ 2 bước đã được kích hoạt thành công cho tài khoản của Quý khách.'";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.TwoStepsActivated,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(twoStepsActivated);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_mobile_added_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            MobileAdded mobileAdded = new MobileAdded(id)
            {
                UserId = 123,
            };

            string template = "Thêm số điện thoại";
            string content = "Thêm số điện thoại";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.MobileAdded,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(mobileAdded);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }

        [TestMethod]
        public void when_receive_email_added_then_generate_content_and_insert()
        {
            //Prepare
            Guid id = Guid.NewGuid();
            EmailAdded emailAdded = new EmailAdded(id)
            {
                UserId = 123,
            };

            string template = "Thêm email";
            string content = "Thêm email";

            using (var context = new MobileNotificationDbContext(dbName))
            {
                context.MessageTemplates.Add(new MessageTemplate()
                {
                    ResourceType = (int)ResourceTypes.User,
                    ResourceState = (int)UserState.EmailAdded,
                    Template = template
                });

                context.SaveChanges();
            }

            //Process
            handler.Handle(emailAdded);

            //Check
            using (var context = new MobileNotificationDbContext(dbName))
            {
                var mobileMessage = context.MobileMessages.First();
                Assert.IsTrue(mobileMessage.Message == content);
            }
        }
        #endregion
    }
}
