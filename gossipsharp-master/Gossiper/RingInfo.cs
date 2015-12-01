using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gossiper
{
    public class RingInfo
    {
        public IPEndPoint IP { get; set; }
        public bool Status { get; set; }
    }
}