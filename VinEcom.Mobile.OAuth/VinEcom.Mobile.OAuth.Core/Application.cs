using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core
{
    public class Application
    {
        public Guid AppId { get; set; }
        public string AppSecret { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public bool IsActive { get; set; }
    }
}