using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus;
using ADR.Mobile.Infrastructure.Redis;
using ADR.Mobile.Infrastructure.Settings;

namespace VinEcom.Mobile.OAuth.Service
{
    [XmlRoot("InfrastructureSettings", Namespace = XmlNamespace)]
    public class MobileOAuthSettings : InfrastructureSettings
    {
        public ServiceBusSettings ServiceBus { get; set; }
        public RedisCacheSetting RedisCache { get; set; }
    }
}
