using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Mappings
{
    public class UserDeviceMap : EntityTypeConfiguration<UserDevice>
    {
        public UserDeviceMap()
        {
            ToTable("UserDevice");
            HasKey(t => t.Id);
            Ignore(t => t.NeedLogin);
            Ignore(t => t.NeedRefreshToken);
        }
    }
}
