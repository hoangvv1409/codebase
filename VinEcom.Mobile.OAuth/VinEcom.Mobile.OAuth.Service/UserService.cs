using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjectPublicUser;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.UserPublicClient;

namespace VinEcom.Mobile.OAuth.Service
{
    public class UserService : IUserService
    {
        private IUserClient client;

        public UserService(IUserClient userClient)
        {
            this.client = userClient;
        }

        public IEnumerable<ClaimModel> Login(string email, string password, out string message)
        {
            int status = 0;
            var request = new UserLoginRequest() { Email = email, Password = password };
            var userInfo = client.Login(request, out status, out message);

            if (userInfo != null && userInfo.UserId > 0)
            {
                var claims = new List<ClaimModel>();
                var properties = userInfo.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    string att = prop.Name;
                    string value = string.Empty;

                    if (prop.GetValue(userInfo, null) != null)
                    {
                        value = prop.PropertyType != typeof(Dictionary<string, string>)
                            ? prop.GetValue(userInfo, null).ToString()
                            : "";
                    }

                    claims.Add(new ClaimModel(att, value));
                }

                return claims;
            }

            return null;
        }
    }
}
