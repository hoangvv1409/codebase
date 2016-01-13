using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public class MobileMessage
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long SOID { get; set; }
        public long ShipmentId { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int SendStatus { get; set; }
        public int SeenStatus { get; set; }
        public short Type { get; set; }
        public DateTime CreatedTime { get; set; }
        public long AMT { get; set; }

        public void Init()
        {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.Now;
            SeenStatus = 0;
            SendStatus = 0;
        }
    }
}
