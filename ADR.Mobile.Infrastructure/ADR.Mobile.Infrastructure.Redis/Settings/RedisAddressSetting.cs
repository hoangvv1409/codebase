using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADR.Mobile.Infrastructure.Settings;

namespace ADR.Mobile.Infrastructure.Redis
{
    [XmlRoot("Address", Namespace = InfrastructureSettings.XmlNamespace)]
    public class RedisAddressSetting
    {
        public string ServerIP { get; set; }
        public int DbId { get; set; }
    }
}
