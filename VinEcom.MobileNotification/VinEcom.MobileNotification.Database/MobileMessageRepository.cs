using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database
{
    public class MobileMessageRepository : Repository<MobileMessage>
    {
        public MobileMessageRepository(Func<MobileNotificationDbContext> dbContextFactory)
            : base(dbContextFactory)
        { }

        public IEnumerable<MobileMessage> GetHistory(int userId, int pageIndex, int pageSize)
        {
            return null;
        }

        public int GetUnreadMessageNumber(long userId)
        {
            return this.dbContext.MobileMessage.Count(m => m.UserId == userId && m.SeenStatus == 0);
        }

        public void SetUnredMessageToRead(long userId)
        {
            var mobileMsgs = this.dbContext.MobileMessage.Where(m => m.UserId == userId && m.SeenStatus == 0).ToList();
            
        }
    }
}
