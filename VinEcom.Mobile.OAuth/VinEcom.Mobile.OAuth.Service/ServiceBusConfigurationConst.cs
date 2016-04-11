using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Service
{
    public static class Topics
    {
        public static class Events
        {
            public const string Path = "authen/events";

            public static class Subscriptions
            {
                public const string OAuthViewGenerator = "OAuthViewGenerator";
                public const string AuthenInfoSync = "AuthenInfoSync";
            }
        }

        public static class Commands
        {
            public const string Path = "authen/commands";

            public static class Subscriptions
            {
                public const string OAuthViewGenerator = "OAuthViewGenerator";
            }
        }
    }
}
