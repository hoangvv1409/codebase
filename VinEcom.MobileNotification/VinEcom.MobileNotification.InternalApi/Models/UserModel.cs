using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinEcom.MobileNotification.InternalApi.Models
{
    public class UserModel
    {
        public long UserId { get; set; }
        public decimal? AdrPoint { get; set; }
        public long SOID { get; set; }
        public int UserState { get; set; }
    }
}