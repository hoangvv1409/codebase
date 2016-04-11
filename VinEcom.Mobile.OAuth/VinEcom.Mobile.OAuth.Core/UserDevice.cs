using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class UserDevice
    {
        public long Id { get; private set; }
        public string ClientId { get; private set; }
        public long UserId { get; private set; }
        public bool NeedLogin { get; private set; }
        public bool NeedRefreshToken { get; private set; }

        protected UserDevice()
        { }

        public UserDevice(long userId, string clientId)
        {
            this.UserId = userId;
            this.ClientId = clientId;
            this.NeedLogin = this.NeedRefreshToken = false;
        }

        public void SetNeedRefreshTokenTo(bool needRefreshToken)
        {
            this.NeedRefreshToken = needRefreshToken;
        }

        public void SetNeedLoginTo(bool needLogin)
        {
            this.NeedLogin = needLogin;
        }
    }
}
