using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Mappings
{
    public class AdminUserClaimMap : EntityTypeConfiguration<UserClaim>
    {
        public AdminUserClaimMap()
        {
            ToTable("UserClaim", Schema.AdminSchema);
            HasKey(t => t.Id);
        }
    }
}
