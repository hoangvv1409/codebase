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
    public class ShipmentEventHandler :
        IEventHandler<ShipmentArrived>,
        IEventHandler<ShipmentShipBegun>
    {
        private IContentGenerator contentGenerator;
        private Func<MobileNotificationDbContext> dbContext;

        public ShipmentEventHandler(Func<MobileNotificationDbContext> dbContext, IContentGenerator contentGenerator)
        {
            this.contentGenerator = contentGenerator;
            this.dbContext = dbContext;
        }

        public void Handle(ShipmentArrived e)
        {
            GenerateContent(ShipmentState.Arrived, e, (content) => new MobileMessage()
            {
                ShipmentId = e.ShipmentId,
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(ShipmentShipBegun e)
        {
            GenerateContent(ShipmentState.Shipping, e, (content) => new MobileMessage()
            {
                ShipmentId = e.ShipmentId,
                SOID = e.SOID,
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        private void GenerateContent(ShipmentState state, IEvent e, Func<string, MobileMessage> bindingFunction)
        {
            string template = this.GetTemplate(state);
            string content = contentGenerator.Generate(template, e);

            MobileMessage mobileMessage = bindingFunction.Invoke(content);

            this.SaveMobileMessage(mobileMessage);
        }

        private string GetTemplate(ShipmentState state)
        {
            MessageTemplateRepository repository = new MessageTemplateRepository(dbContext);

            return repository.GetTemplateByStateAndResourceType((int)ResourceTypes.Shipment, (int)state);
        }

        private void SaveMobileMessage(MobileMessage mobileMessage)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            repository.Save(mobileMessage);
            repository.SaveChanges();
        }
    }
}
