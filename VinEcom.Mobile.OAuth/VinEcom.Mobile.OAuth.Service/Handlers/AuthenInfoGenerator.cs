using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using VinEcom.Mobile.Authen.Contracts.Events;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Database.Repository;

namespace VinEcom.Mobile.OAuth.Service.Handlers
{
    public class AuthenInfoGenerator :
        IEventHandler<UserAdminCreated>,
        IEventHandler<UserAdminClaimAdded>,
        IEventHandler<UserAdminClaimRemoved>,
        IEventHandler<AppCreated>,
        IEventHandler<AppClaimAdded>,
        IEventHandler<AppClaimRemoved>,
        IEventHandler<AppSecretChanged>,
        IEventHandler<UserAdminPasswordChanged>
    {
        private Func<OAuthDbContext> dbContext;

        public AuthenInfoGenerator(Func<OAuthDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Handle(UserAdminCreated @event)
        {
            var repository = new Repository<AdminUser>(this.dbContext);

            var adminUser = new AdminUser()
            {
                Email = @event.Email,
                Password = @event.Password,
                CreatedDate = @event.CreatedDate,
                CreatedUser = @event.CreatedUser,
                IsActive = @event.IsActive
            };

            repository.Save(adminUser);
            repository.SaveChanges();
        }

        public void Handle(UserAdminClaimAdded @event)
        {
            var repository = new Repository<UserClaim>(this.dbContext);

            var userClaim = new UserClaim()
            {
                Email = @event.Email,
                ClaimType = @event.ClaimType,
                ClaimValue = @event.ClaimValue
            };

            repository.Save(userClaim);
            repository.SaveChanges();
        }

        public void Handle(UserAdminClaimRemoved @event)
        {
            var userClaim = new UserClaim()
            {
                Email = @event.Email,
                ClaimType = @event.ClaimType,
                ClaimValue = @event.ClaimValue
            };

            this.dbContext.Invoke().AdminUserClaims.Attach(userClaim);
            this.dbContext.Invoke().AdminUserClaims.Remove(userClaim);
            this.dbContext.Invoke().SaveChanges();
        }

        public void Handle(AppCreated @event)
        {
            var repository = new Repository<Application>(this.dbContext);

            var app = new Application()
            {
                AppId = @event.AppId,
                AppSecret = @event.AppSecret,
                CreatedDate = @event.CreatedDate,
                CreatedUser = @event.CreatedUser,
                IsActive = @event.IsActive
            };

            repository.Save(app);
            repository.SaveChanges();
        }

        public void Handle(AppClaimAdded @event)
        {
            var repository = new Repository<ApplicationClaim>(this.dbContext);

            var appClaim = new ApplicationClaim()
            {
                AppId = @event.AppId,
                ClaimType = @event.ClaimType,
                ClaimValue = @event.ClaimValue
            };

            repository.Save(appClaim);
            repository.SaveChanges();
        }

        public void Handle(AppClaimRemoved @event)
        {
            var appClaim = new ApplicationClaim()
            {
                AppId = @event.AppId,
                ClaimType = @event.ClaimType,
                ClaimValue = @event.ClaimValue
            };

            this.dbContext.Invoke().ApplicationClaims.Attach(appClaim);
            this.dbContext.Invoke().ApplicationClaims.Remove(appClaim);
            this.dbContext.Invoke().SaveChanges();
        }

        public void Handle(AppSecretChanged @event)
        {
            var repository = new Repository<Application>(this.dbContext);

            var app = repository.Find(@event.AppId);
            app.AppSecret = @event.AppSecret;

            repository.SaveChanges();
        }

        public void Handle(UserAdminPasswordChanged @event)
        {
            var repository = new Repository<AdminUser>(this.dbContext);

            var adminUser = repository.Find(@event.Email);
            adminUser.Password = @event.Password;

            repository.SaveChanges();
        }
    }
}
