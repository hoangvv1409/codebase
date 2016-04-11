using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class ApplicationClaim
    {
        public int Id { get; set; }
        public Guid AppId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
