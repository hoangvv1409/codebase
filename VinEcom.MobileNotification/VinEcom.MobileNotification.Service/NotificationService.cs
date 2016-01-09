using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Database;

namespace VinEcom.MobileNotification.Service
{
    public class NotificationService
    {
        private Func<MobileNotificationDbContext> dbContext;

        public NotificationService(Func<MobileNotificationDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public void PushOrderNotification(OrderNotification orderNotification)
        {
            orderNotification.GenerateEvent();
            OrderNotificationRepository repository = new OrderNotificationRepository(dbContext);
            repository.Save(orderNotification);
            repository.SaveChanges();

            //TODO Save Undispatch db then send to bus
        }

        public void PushShipmentNotification(ShipmentNotification shipmentNotification)
        {
            shipmentNotification.GenerateEvent();
            ShipmentNotificationRepository repository = new ShipmentNotificationRepository(dbContext);
            repository.Save(shipmentNotification);
            repository.SaveChanges();

            //TODO Save Undispatch db then send to bus
        }

        public void PushUserNotification(UserNotification userNotification)
        {
            userNotification.GenerateEvent();
            UserNotificationRepository repository = new UserNotificationRepository(dbContext);
            repository.Save(userNotification);
            repository.SaveChanges();

            //TODO Save Undispatch db then send to bus
        }

        public void SaveMobileMessage(MobileMessage mobileMessage)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);
            repository.Save(mobileMessage);
            repository.SaveChanges();
        }

        public IEnumerable<MobileMessage> GetUnreadMessages(int userId, int pageIndex, int pageSize)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            return repository.GetHistory(userId, pageIndex, pageSize);
        }

        public int CountUnreadMessage(long userId)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            var count = repository.GetUnreadMessageNumber(userId);
            repository.SetUnredMessageToRead(userId);
            repository.SaveChanges();

            return count;
        }
    }
}
