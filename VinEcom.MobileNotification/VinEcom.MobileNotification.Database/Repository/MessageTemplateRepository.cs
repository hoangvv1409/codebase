using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database
{
    public class MessageTemplateRepository : Repository<MessageTemplate>
    {
        public MessageTemplateRepository(Func<MobileNotificationDbContext> dbContextFactory)
            : base(dbContextFactory)
        { }

        public string GetTemplateByStateAndResourceType(int resourceType, int state)
        {
            return this.dbContext.MessageTemplates.First(
                    m => m.ResourceType == resourceType && m.ResourceState == state).Template;
        }
    }
}
