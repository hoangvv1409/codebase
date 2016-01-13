using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Database.Mappings;

namespace VinEcom.MobileNotification.Database
{
    public class MobileNotificationDbContext : DbContext
    {
        public MobileNotificationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Orders = base.Set<Order>();
            this.Shipments = base.Set<Shipment>();
            this.Users = base.Set<User>();
            this.MobileMessages = base.Set<MobileMessage>();
            this.MessageTemplates = base.Set<MessageTemplate>();
            this.EventMessages = base.Set<EventMessage>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MobileMessageMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new ShipmentMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new MessageTemplateMap());
            modelBuilder.Configurations.Add(new EventMessageMap());
        }

        public DbSet<Order> Orders { get; private set; }
        public DbSet<Shipment> Shipments { get; private set; }
        public DbSet<User> Users { get; private set; }
        public DbSet<MobileMessage> MobileMessages { get; private set; }
        public DbSet<MessageTemplate> MessageTemplates { get; private set; }
        public DbSet<EventMessage> EventMessages { get; private set; }
    }
}
