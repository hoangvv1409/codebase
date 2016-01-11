using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database.Mappings
{
    public class EventMessageMap : EntityTypeConfiguration<EventMessage>
    {
        public EventMessageMap()
        {
            ToTable("EventMessages", Schema.MessagingSchema);
            HasKey(t => t.Id);
        }
    }
}
