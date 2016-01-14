using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NotificationGenerateHandler :
        IEventHandler<OrderArrived>,
        IEventHandler<OrderCancelled>,
        IEventHandler<OrderConfirmed>,
        IEventHandler<OrderPaided>,
        IEventHandler<OrderPartiallyCancelled>,
        IEventHandler<OrderShipBegun>,
        IEventHandler<ShipmentArrived>,
        IEventHandler<ShipmentShipBegun>,
        IEventHandler<AdrPointsUsed>,
        IEventHandler<AdrPointAdded>,
        IEventHandler<AdrPointsRefunded>,
        IEventHandler<EmailAdded>,
        IEventHandler<MobileAdded>,
        IEventHandler<PasswordChanged>,
        IEventHandler<TwoStepsActivated>
    {
        private IContentGenerator contentGenerator;
        private Func<MobileNotificationDbContext> dbContext;

        public NotificationGenerateHandler(Func<MobileNotificationDbContext> dbContext, IContentGenerator contentGenerator)
        {
            this.contentGenerator = contentGenerator;
            this.dbContext = dbContext;
        }

        #region Order
        public void Handle(OrderArrived e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.Arrived, e, (content) => new MobileMessage()
            {
                Address = e.WarehouseName,
                Id = e.SourceId,
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderCancelled e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.Cancel, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderConfirmed e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.Confirm, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderPaided e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.Paid, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderPartiallyCancelled e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.PartiallyCancel, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(OrderShipBegun e)
        {
            GenerateContent((int)ResourceTypes.Order, (int)OrderState.Shipping, e, (content) => new MobileMessage()
            {
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }
        #endregion

        #region Shipment
        public void Handle(ShipmentArrived e)
        {
            GenerateContent((int)ResourceTypes.Shipment, (int)ShipmentState.Arrived, e, (content) => new MobileMessage()
            {
                Address = e.WarehouseName,
                ShipmentId = e.ShipmentId,
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(ShipmentShipBegun e)
        {
            GenerateContent((int)ResourceTypes.Shipment, (int)ShipmentState.Shipping, e, (content) => new MobileMessage()
            {
                ShipmentId = e.ShipmentId,
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }
        #endregion

        #region User
        public void Handle(AdrPointsUsed e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.AdrPointsUsed, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content,
                SOID = e.SOID
            });
        }

        public void Handle(AdrPointAdded e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.AdrPointAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(AdrPointsRefunded e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.AdrPointsRefunded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content,
                SOID = e.SOID
            });
        }

        public void Handle(EmailAdded e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.EmailAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(MobileAdded e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.MobileAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(PasswordChanged e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.PasswordChanged, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(TwoStepsActivated e)
        {
            GenerateContent((int)ResourceTypes.User, (int)UserState.TwoStepsActivated, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }
        #endregion

        private void GenerateContent(int resourceType, int state, IEvent e, Func<string, MobileMessage> bindingFunction)
        {
            MobileMessage mobileMessage = GetMessageById(e.SourceId);

            if (mobileMessage == null)
            {
                string template = this.GetTemplate(resourceType, state);
                string content = contentGenerator.Generate(template, e);

                mobileMessage = bindingFunction.Invoke(content);
                mobileMessage.Id = e.SourceId;
                mobileMessage.ResourceState = (short)state;
                mobileMessage.ResourceType = (short)resourceType;

                this.SaveMobileMessage(mobileMessage);
            }

            //TODO Send to OneSignal
        }

        private string GetTemplate(int resourceType, int state)
        {
            MessageTemplateRepository repository = new MessageTemplateRepository(dbContext);

            return repository.GetTemplateByStateAndResourceType(resourceType, state);
        }

        private MobileMessage GetMessageById(Guid id)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            return repository.GetMessageById(id);
        }

        private void SaveMobileMessage(MobileMessage mobileMessage)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            repository.Save(mobileMessage);
            repository.SaveChanges();
        }
    }
}
