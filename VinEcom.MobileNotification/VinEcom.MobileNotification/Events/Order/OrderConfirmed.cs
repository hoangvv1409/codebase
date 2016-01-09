using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Events
{
    public class OrderConfirmed : IEvent
    {
        public long SOID { get; set; }
        public int UserId { get; set; }
        public OrderState OrderState
        {
            get
            {
                return OrderState.Confirm;
            }
        }
    }
}
