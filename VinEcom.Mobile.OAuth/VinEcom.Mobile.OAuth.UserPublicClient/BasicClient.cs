using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.UserPublicClient
{
    public interface IBasicClient
    {
        string ResponseMessage { get; set; }
        int Status { get; set; }
        string SetResponseMessage(Exception ex);
        int Uid { get; set; }
    }

    public class BasicClient : IBasicClient
    {
        #region Settings

        static readonly string UserApiUri = ConfigurationManager.AppSettings["UserApi.Uri"];
        public static string Domain = string.IsNullOrEmpty(UserApiUri) ? "http://10.220.48.112:8092/publicapi.ashx?type=" : UserApiUri;
        public static string GetUrl(BusinessObjectPublicUser.RequestFunction function)
        {
            return string.Format("{0}/{1}", Domain, function);
        }

        public static string Appid = ConfigurationManager.AppSettings["UserApi.AppID"];
        public static string ConsumerSecret = ConfigurationManager.AppSettings["UserApi.ConsumerSecret"];
        public static string Publickey = ConfigurationManager.AppSettings["UserApi.RSAPublickey"];

        #endregion

        #region IBasicClient Implementation
        public string ResponseMessage { get; set; }
        public int Status { get; set; }
        public int Uid { get; set; }
        public string SetResponseMessage(Exception ex)
        {
            string r = ex.Message;
            if (ex.InnerException != null)
                r += " | " + ex.InnerException.Message;
            return r;
        }

        #endregion
    }
}
