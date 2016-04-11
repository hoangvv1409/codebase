using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class UserClaim
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}