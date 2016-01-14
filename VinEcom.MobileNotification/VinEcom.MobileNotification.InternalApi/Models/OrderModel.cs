using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class OrderModel
    {
        [Required(ErrorMessage = "SOID is required")]
        public long SOID { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "OrderState is required")]
        public int OrderState { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}