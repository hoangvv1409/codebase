using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database
{
    public class MobileNotificationDbContext : DbContext
    {
        public const string SchemaName = "Notification";
        public MobileNotificationDbContext(string connectionString)
            : base(connectionString)
        {
            this.OrderNotification = base.Set<OrderNotification>();
            this.ShipmentNotification = base.Set<ShipmentNotification>();
            this.UserNotification = base.Set<UserNotification>();
            this.MobileMessage = base.Set<MobileMessage>();
            this.EventMessage = base.Set<EventMessage>();
        }

        public DbSet<OrderNotification> OrderNotification { get; private set; }
        public DbSet<ShipmentNotification> ShipmentNotification { get; private set; }
        public DbSet<UserNotification> UserNotification { get; private set; }
        public DbSet<MobileMessage> MobileMessage { get; private set; }
        public DbSet<EventMessage> EventMessage { get; private set; }
    }
}
