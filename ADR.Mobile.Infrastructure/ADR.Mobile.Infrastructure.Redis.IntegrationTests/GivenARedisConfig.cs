using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Redis.IntegrationTests
{
    public class GivenARedisConfig
    {
        private const int IoTimeOut = 100000;
        private const int SyncTimeout = 100000;


        protected RedisConnectionSetting ConnectionSettings { get; private set; }
        protected RedisAddressSetting ServerAddressSettings { get; private set; }
        protected RedisCacheSetting RedisCacheSetting { get; set; }

        public GivenARedisConfig()
        {
            this.ServerAddressSettings = new RedisAddressSetting()
            {
                ServerIP = "10.220.75.22:6380",
                DbId = 11
            };

            this.ConnectionSettings = new RedisConnectionSetting()
            {
                KeepAlive = 5,
                SyncTimeout = SyncTimeout,
                AbortOnConnectionFail = false,
                AllowAdmin = true,
                ConnectionTimeout = IoTimeOut,
                ConnectRetry = 5
            };

            this.RedisCacheSetting = new RedisCacheSetting()
            {
                ConnectionSettings = this.ConnectionSettings,
                RedisMaster = this.ServerAddressSettings,
                RedisSlave = this.ServerAddressSettings
            };
        }
    }
}
