using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class UserModel
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public int UserState { get; set; }
        public decimal? AdrPoint { get; set; }
        public long? SOID { get; set; }
    }
}