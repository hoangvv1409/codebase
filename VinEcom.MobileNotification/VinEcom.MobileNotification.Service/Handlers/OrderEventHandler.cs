using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Service.Handlers
{
    public class OrderEventHandler :
        IEventHandler<OrderArrived>,
        IEventHandler<OrderCancelled>,
        IEventHandler<OrderConfirmed>,
        IEventHandler<OrderPaided>,
        IEventHandler<OrderPartiallyCancelled>,
        IEventHandler<OrderShipBegun>
    {
        private IContentGenerator contentGenerator;
        private Func<MobileNotificationDbContext> dbContext;

        public OrderEventHandler(Func<MobileNotificationDbContext> dbContext, IContentGenerator contentGenerator)
        {
            this.contentGenerator = contentGenerator;
            this.dbContext = dbContext;
        }

        public void Handle(OrderArrived e)
        {
            GenerateContent(OrderState.Arrived, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderCancelled e)
        {
            GenerateContent(OrderState.Cancel, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderConfirmed e)
        {
            GenerateContent(OrderState.Confirm, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderPaided e)
        {
            GenerateContent(OrderState.Paid, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderPartiallyCancelled e)
        {
            GenerateContent(OrderState.PartiallyCancel, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderShipBegun e)
        {
            GenerateContent(OrderState.Shipping, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        private void GenerateContent(OrderState state, IEvent e, Func<string, MobileMessage> bindingFunction)
        {
            string template = this.GetTemplate(state);
            string content = contentGenerator.Generate(template, e);

            MobileMessage mobileMessage = bindingFunction.Invoke(content);

            this.SaveMobileMessage(mobileMessage);
        }

        private string GetTemplate(OrderState state)
        {
            MessageTemplateRepository repository = new MessageTemplateRepository(dbContext);

            return repository.GetTemplateByStateAndResourceType((int)ResourceTypes.Order, (int)state);
        }

        private void SaveMobileMessage(MobileMessage mobileMessage)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            repository.Save(mobileMessage);
            repository.SaveChanges();
        }
    }
}