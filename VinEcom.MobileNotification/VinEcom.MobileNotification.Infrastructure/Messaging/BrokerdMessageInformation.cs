using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging
{
    public class BrokerdMessageInformation
    {
        public string MessageId { get; set; }
        public string SessionId { get; set; }
        public string CorrelationId { get; set; }
        public int DeliveryCount { get; set; }
        public DateTime EnqueuedTime { get; set; }
    }
}
