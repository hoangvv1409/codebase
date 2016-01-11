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
using VinEcom.MobileNotification.Service.Handlers;

namespace VinEcom.MobileNotification.Service.Tests
{
    [TestClass]
    public class UserEventHandlerIntergrationFixture : GivenADbContext
    {
        private UserEventHandler handler;
        private int resourceType = (int)ResourceTypes.User;

        public UserEventHandlerIntergrationFixture()
        {
            IContentGenerator contentGenerator = new ContentGenerator();
            this.handler = new UserEventHandler(() => new MobileNotificationDbContext(dbName), contentGenerator);
        }

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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
                    ResourceType = resourceType,
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
    }
}
