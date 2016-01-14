using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class ShipmentModel
    {
        [Required]
        public int ShipmentId { get; set; }
        [Required]
        public long SOID { get; set; }
        [Required]
        public int ShipmentState { get; set; }
        [Required]
        public long UserId { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}