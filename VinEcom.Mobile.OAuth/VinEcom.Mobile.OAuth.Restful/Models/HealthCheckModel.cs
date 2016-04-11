using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinEcom.Mobile.OAuth.Restful.Models
{
    public class HealthCheckModel
    {
        public string Version { get; set; }
        public bool ConnectDBStatus { get; set; }
    }
}