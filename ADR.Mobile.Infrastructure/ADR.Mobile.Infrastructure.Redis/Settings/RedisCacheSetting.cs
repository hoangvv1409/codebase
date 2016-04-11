using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADR.Mobile.Infrastructure.Settings;

namespace ADR.Mobile.Infrastructure.Redis
{
    [XmlRoot("Redis", Namespace = InfrastructureSettings.XmlNamespace)]
    public class RedisCacheSetting
    {
        public RedisConnectionSetting ConnectionSettings { get; set; }
        public RedisAddressSetting RedisMaster { get; set; }
        public RedisAddressSetting RedisSlave { get; set; }
        public RedisAddressSetting RedisSession { get; set; }
        public RedisAddressSetting RedisConsumer { get; set; }
    }
}
