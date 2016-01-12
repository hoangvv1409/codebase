using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.MobileNotification.InternalApi.Models;

namespace VinEcom.MobileNotification.InternalApi.Controllers
{
    public class UserController : ApiController
    {
        [Route("notification/users")]
        public void Post([FromBody]UserModel user)
        {
        }
    }
}
