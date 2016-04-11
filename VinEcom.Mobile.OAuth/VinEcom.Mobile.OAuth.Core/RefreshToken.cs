using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class RefreshToken
    {
        public Guid RefreshTokenValue { get; private set; }
        public string ClientId { get; private set; }
        public string AuthenticationTicket { get; private set; }
        public DateTimeOffset CreatedTime { get; private set; }
        public DateTimeOffset ExpiredTime { get; private set; }

        public RefreshToken(string clientId, int expiredTimeInMinutes)
            : this(Guid.NewGuid(), clientId, expiredTimeInMinutes)
        { }

        public RefreshToken(Guid refreshToken, string clientId, int expiredTimeInMinutes)
        {
            this.RefreshTokenValue = refreshToken;
            this.ClientId = clientId;
            this.CreatedTime = DateTime.UtcNow;
            this.ExpiredTime = DateTime.UtcNow.AddMinutes(expiredTimeInMinutes);
        }

        public void SetAuthenticationTicket(string authenticationTicket)
        {
            this.AuthenticationTicket = authenticationTicket;
        }
    }
}
