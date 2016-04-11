using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public class AdminUserRepository : IAdminUserRepository
    {
        protected OAuthDbContext dbContext;

        public AdminUserRepository(Func<OAuthDbContext> dbContextFactory)
        {
            this.dbContext = dbContextFactory.Invoke();
        }

        public bool IsExist(string email, string password)
        {
            return this.dbContext.AdminUsers.FirstOrDefault(u => u.Email == email && u.Password == password && u.IsActive) != null;
        }

        public IEnumerable<UserClaim> GetAdminUserClaims(string email)
        {
            return this.dbContext.AdminUserClaims.Where(u => u.Email == email);
        }
    }
}
