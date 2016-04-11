using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Redis;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Core;
using Newtonsoft.Json;

namespace VinEcom.Mobile.OAuth.Service
{
    public class RedisDeviceService : IDeviceService
    {
        private RedisReadClient redisReadClient;
        private string UserDevicePrefixNamespace;

        public RedisDeviceService(RedisReadClient redisReadClient)
        {
            this.redisReadClient = redisReadClient;
            this.UserDevicePrefixNamespace = "UserDevice";
        }

        public UserDevice GetDevice(long userId, string deviceId)
        {
            var key = string.Format("{0}:{1}:{2}", this.UserDevicePrefixNamespace, userId, deviceId);
            var userDeviceJson = this.redisReadClient.Get(key);

            return JsonConvert.DeserializeObject<UserDevice>(userDeviceJson);
        }
    }
}
