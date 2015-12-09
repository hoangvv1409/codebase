using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ScheduleController : ApiController
    {
        /// <summary>
        /// Get all Merchant Schedules
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("merchants/schedules")]
        public IEnumerable<Schedule> GetSchedules()
        {
            var schedules = new List<Schedule>();

            return schedules;
        }

        /// <summary>
        /// Get all Schedules from specific Merchant
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("merchants/{merchantId}/schedules")]
        public IEnumerable<Schedule> GetSchedulesByMerchant(int merchantId)
        {
            var schedules = new List<Schedule>();

            return schedules;
        }
    }
}
