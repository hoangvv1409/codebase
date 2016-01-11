using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Database;

namespace VinEcom.MobileNotification.Service.Tests
{
    public class GivenADbContext : IDisposable
    {
        protected string dbName;

        public GivenADbContext()
        {
            dbName = this.GetType().Name + "-" + Guid.NewGuid().ToString();
            using (var context = new MobileNotificationDbContext(dbName))
            {
                if (context.Database.Exists())
                    context.Database.Delete();

                context.Database.Create();
            }
        }

        public void Dispose()
        {
            using (var context = new MobileNotificationDbContext(dbName))
            {
                if (context.Database.Exists())
                    context.Database.Delete();
            }
        }
    }
}
