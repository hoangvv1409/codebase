using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.InternalApi.Models;
using VinEcom.MobileNotification.Service;

namespace VinEcom.MobileNotification.InternalApi.Controllers
{
    public class UserController : ApiController
    {
        private NotificationService notificationService;

        public UserController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [Route("notification/users")]
        public void Post([FromBody]UserModel userModel)
        {
            if (!ModelState.IsValid) return;
            if (userModel.UserState >= 5)
            {
                if (userModel.AdrPoint == null)
                {
                    return;
                }

                if (userModel.UserState >= 6 && userModel.SOID == null)
                {
                    return;
                }
            }

            User user = null;
            switch (userModel.UserState)
            {
                case 1:
                    user = new User(userModel.UserId, UserState.PasswordChanged);
                    break;
                case 2:
                    user = new User(userModel.UserId, UserState.TwoStepsActivated);
                    break;
                case 3:
                    user = new User(userModel.UserId, UserState.EmailAdded);
                    break;
                case 4:
                    user = new User(userModel.UserId, UserState.MobileAdded);
                    break;
                case 5:
                    user = new User(userModel.UserId, UserState.AdrPointAdded, (decimal)userModel.AdrPoint);
                    break;
                case 6:
                    user = new User(userModel.UserId, UserState.AdrPointsUsed, (decimal)userModel.AdrPoint, (long)userModel.SOID);
                    break;
                case 7:
                    user = new User(userModel.UserId, UserState.AdrPointsRefunded, (decimal)userModel.AdrPoint, (long)userModel.SOID);
                    break;
                default:
                    user = null;
                    return;
            }

            this.notificationService.PushUserNotification(user);
        }
    }
}
