using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.MobileNotification.Api.Models;

namespace VinEcom.MobileNotification.Api.Controllers
{
    public class NotificationController : ApiController
    {
        [Route("notifications")]
        public IEnumerable<NotificationModel> Get(int pageIndex, int pageSize)
        {
            return new List<NotificationModel>();
        }

        [HttpGet]
        [Route("notifications/count")]
        public int CountUnread()
        {
            return 0;
        }

        [Route("notifications/{id}")]
        public NotificationModel Get(int id)
        {
            return new NotificationModel();
        }
    }
}
