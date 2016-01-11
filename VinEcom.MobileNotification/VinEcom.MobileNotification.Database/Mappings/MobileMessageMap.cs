using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database.Mappings
{
    public class MobileMessageMap : EntityTypeConfiguration<MobileMessage>
    {
        public MobileMessageMap()
        {
            ToTable("MobileMessage", Schema.MobileNotificationSchema);
            HasKey(t => t.Id);
        }
    }
}
