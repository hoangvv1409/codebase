﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.MobileNotification.InternalApi.Models;

namespace VinEcom.MobileNotification.InternalApi.Controllers
{
    public class OrderController : ApiController
    {
        // POST api/order
        [Route("notification/orders")]
        public void Post([FromBody]OrderModel order)
        {
        }
    }
}
