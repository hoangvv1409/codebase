using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class OrderModel
    {
        public long SOID { get; set; }
        public long UserId { get; set; }
        public int OrderState { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}