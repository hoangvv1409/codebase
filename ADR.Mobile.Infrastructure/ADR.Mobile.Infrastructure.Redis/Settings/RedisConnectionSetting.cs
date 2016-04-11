using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADR.Mobile.Infrastructure.Settings;

namespace ADR.Mobile.Infrastructure.Redis
{
    [XmlRoot("Connection", Namespace = InfrastructureSettings.XmlNamespace)]
    public class RedisConnectionSetting
    {
        public int KeepAlive { get; set; }
        public int SyncTimeout { get; set; }
        public bool AbortOnConnectionFail { get; set; }
        public bool AllowAdmin { get; set; }
        public int ConnectionTimeout { get; set; }
        public int ConnectRetry { get; set; }
    }
}
