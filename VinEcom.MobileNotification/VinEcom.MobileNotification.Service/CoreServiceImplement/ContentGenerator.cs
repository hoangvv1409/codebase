using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Service
{
    public class ContentGenerator : IContentGenerator
    {
        public string Generate(string template, IEvent e)
        {
            var properties = e.GetType().GetProperties();

            //TODO Change to RegexReplace
            foreach (var prop in properties)
            {
                if (!Regex.IsMatch(template, "\\{.+\\}+")) break;
                string att = prop.Name;
                string value = prop.GetValue(e, null).ToString();
                template = Regex.Replace(template, "\\{" + att + "\\}+", value, RegexOptions.IgnoreCase);
            }

            return template;
        }
    }
}
