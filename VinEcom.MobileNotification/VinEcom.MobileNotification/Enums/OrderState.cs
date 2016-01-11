using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Enums
{
    public enum OrderState
    {
        Confirm = 1,
        Cancel = 2,
        Paid = 3,
        Shipping = 4,
        Arrived = 5,
        PartiallyCancel = 6
    }
}
