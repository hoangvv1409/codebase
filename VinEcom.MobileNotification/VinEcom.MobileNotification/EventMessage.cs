using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification
{
    public class EventMessage
    {
        public string Payload { get; set; }
        public string CorrelationId { get; set; }
    }
}
