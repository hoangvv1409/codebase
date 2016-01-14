using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OneSignalFacade
{
    public class OneSignalModel
    {
        public string app_id { get; set; }
        public object[] tags { get; set; }
        public bool android_background_data { get; set; }
        public object contents { get; set; }
        public object data { get; set; }
    }
}