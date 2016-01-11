using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Database.Repository;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Service
{
    public class NotificationService
    {
        private Func<MobileNotificationDbContext> dbContext;
        private IEventBus bus;
        private ITextSerializer textSerializer;

        public NotificationService(Func<MobileNotificationDbContext> dbContext, IEventBus bus, ITextSerializer textSerializer)
        {
            this.dbContext = dbContext;
            this.bus = bus;
            this.textSerializer = textSerializer;
        }

        public void PushOrderNotification(Order orderNotification)
        {
            OrderRepository repository = new OrderRepository(dbContext);

            orderNotification.GenerateEvent();
            repository.Save(orderNotification);
            repository.SaveChanges();

            this.Publish(orderNotification);
        }

        public void PushShipmentNotification(Shipment shipmentNotification)
        {
            ShipmentRepository repository = new ShipmentRepository(dbContext);

            shipmentNotification.GenerateEvent();
            repository.Save(shipmentNotification);
            repository.SaveChanges();

            this.Publish(shipmentNotification);
        }

        public void PushUserNotification(User userNotification)
        {
            UserRepository repository = new UserRepository(dbContext);

            userNotification.GenerateEvent();
            repository.Save(userNotification);
            repository.SaveChanges();

            this.Publish(userNotification);
        }

        public IEnumerable<MobileMessage> GetUnreadMessages(int userId, int pageIndex, int pageSize)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);
            var mobileMessage = repository.GetHistory(userId, pageIndex, pageSize);
            repository.SetUnredMessageToRead(userId);

            return mobileMessage;
        }

        public int CountUnreadMessage(long userId)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);
            var count = repository.GetUnreadMessageNumber(userId);

            return count;
        }

        /// <summary>
        /// Save then publish
        /// </summary>
        /// <param name="aggregate"></param>
        private void Publish(Aggregate aggregate)
        {
            EventMessageRepository eventRep = new EventMessageRepository(dbContext, bus, textSerializer);
            eventRep.Save(aggregate);
        }
    }
}
