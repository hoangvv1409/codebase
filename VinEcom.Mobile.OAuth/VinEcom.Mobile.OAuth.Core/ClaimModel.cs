using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class ClaimModel
    {
        public string ClaimType { get; private set; }
        public string ClaimValue { get; private set; }

        public ClaimModel(string type, string value)
        {
            this.ClaimType = type;
            this.ClaimValue = value;
        }
    }
}
