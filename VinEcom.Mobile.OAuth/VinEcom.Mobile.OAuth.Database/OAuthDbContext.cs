using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Database.Mappings;

namespace VinEcom.Mobile.OAuth.Database
{
    public class OAuthDbContext : DbContext
    {
        public OAuthDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.AdminUsers = base.Set<AdminUser>();
            this.AdminUserClaims = base.Set<UserClaim>();
            this.Applications = base.Set<Application>();
            this.ApplicationClaims = base.Set<ApplicationClaim>();
            this.UserDevices = base.Set<UserDevice>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdminUserMap());
            modelBuilder.Configurations.Add(new AdminUserClaimMap());
            modelBuilder.Configurations.Add(new ApplicationMap());
            modelBuilder.Configurations.Add(new ApplicationClaimMap());
            modelBuilder.Configurations.Add(new UserDeviceMap());
        }

        public DbSet<UserDevice> UserDevices { get; private set; }
        public DbSet<AdminUser> AdminUsers { get; private set; }
        public DbSet<UserClaim> AdminUserClaims { get; private set; }
        public DbSet<Application> Applications { get; private set; }
        public DbSet<ApplicationClaim> ApplicationClaims { get; private set; }
    }
}
