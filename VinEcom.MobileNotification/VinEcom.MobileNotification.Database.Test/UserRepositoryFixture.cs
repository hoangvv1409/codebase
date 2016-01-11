using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VinEcom.MobileNotification.Database.Test
{
    [TestClass]
    public class UserRepositoryFixture : GivenADbContext
    {

        private Func<MobileNotificationDbContext> dbContext;

        public UserRepositoryFixture()
        {
            this.dbContext =
                () => new MobileNotificationDbContext(dbName);
        }
    }
}
