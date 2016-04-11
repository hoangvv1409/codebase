using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;

namespace VinEcom.Mobile.OAuth.Contracts.Events
{
    public class UserDeviceCreated : IEvent
    {
        public UserDeviceCreated()
        {
            this.SourceId = Guid.NewGuid();
        }

        public Guid SourceId { get; set; }
        public string DeviceId { get; set; }
        public long UserId { get; set; }

        public bool NeedLogin
        {
            get { return false; }
        }

        public bool NeedRefreshToken
        {
            get { return false; }
        }
    }
}
