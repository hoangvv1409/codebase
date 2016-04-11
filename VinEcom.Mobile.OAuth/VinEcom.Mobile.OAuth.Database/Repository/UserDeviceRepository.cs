using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public class UserDeviceRepository : IUserDeviceRepository
    {
        private Func<OAuthDbContext> dbContext;

        public UserDeviceRepository(Func<OAuthDbContext> dbContextFactory)
        {
            this.dbContext = dbContextFactory;
        }

        public IEnumerable<string> GetListClientId(long userId)
        {
            using (var context = this.dbContext.Invoke())
            {
                return context.UserDevices.Where(u => u.UserId == userId).Select(u => u.ClientId).ToList();
            }
        }

        public bool IsExist(long userId, string clientId)
        {
            using (var context = this.dbContext.Invoke())
            {
                return context.UserDevices.FirstOrDefault(u => u.UserId == userId && u.ClientId == clientId) !=
                   null;
            }
        }

        public void Save(UserDevice userDevice)
        {
            using (var context = this.dbContext.Invoke())
            {
                context.UserDevices.Add(userDevice);
                context.SaveChanges();
            }
        }
    }
}
