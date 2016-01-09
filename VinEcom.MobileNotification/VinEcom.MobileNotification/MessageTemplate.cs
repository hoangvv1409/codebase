using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification
{
    public class MessageTemplate
    {
        public int Id { get; set; }
        public int ResourceType { get; set; }
        public int ResourceState { get; set; }
        public string Template { get; set; }
    }
}
