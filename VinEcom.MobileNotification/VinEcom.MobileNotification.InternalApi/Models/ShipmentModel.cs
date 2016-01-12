using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class ShipmentModel
    {
        public int ShipmentId { get; set; }
        public long SOID { get; set; }
        public int ShipmentState { get; set; }
        public long UserId { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
    }
}