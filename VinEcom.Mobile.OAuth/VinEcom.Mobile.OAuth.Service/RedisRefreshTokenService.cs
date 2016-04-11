using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Redis;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;

namespace VinEcom.Mobile.OAuth.Service
{
    public class RedisRefreshTokenService : IRefreshTokenService
    {
        private RedisReadClient redisReadClient;
        private string RefreshTokenPrefixNamespace;

        public RedisRefreshTokenService(RedisReadClient redisReadClient)
        {
            this.redisReadClient = redisReadClient;
            this.RefreshTokenPrefixNamespace = "Ticket";
        }

        public string GetAuthenticationTicket(Guid refreshToken, string clientId)
        {
            var key = string.Format("{0}:{1}:{2}", this.RefreshTokenPrefixNamespace, clientId, refreshToken);

            return this.redisReadClient.Get(key);
        }
    }
}
