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
            return this.dbContext.MobileMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(t => t.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public override void Save(MobileMessage aggregate)
        {
            aggregate.Init();
            base.Save(aggregate);
        }

        public int GetUnreadMessageNumber(long userId)
        {
            return this.dbContext.MobileMessages
                .Count(m => m.UserId == userId && m.SeenStatus == 0);
        }

        public void SetUnredMessageToRead(long userId)
        {
            var mobileMsgs = this.dbContext.MobileMessages
                .Where(m => m.UserId == userId && m.SeenStatus == 0)
                .ToList();

            foreach (var mobileMsg in mobileMsgs)
            {
                mobileMsg.SeenStatus = 1;
            }

            this.dbContext.SaveChanges();
        }
    }
}
