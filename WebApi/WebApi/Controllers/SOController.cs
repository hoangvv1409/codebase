using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class SOController : ApiController
    {
        /// <summary>
        /// Get all SO from specific transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/sos")]
        public IEnumerable<SO> GetSosByTransporter(int transporterId)
        {
            var sos = new List<SO>();

            return sos;
        }

        /// <summary>
        /// Get specific SO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="soId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/sos/{soId}")]
        public SO GetSoByTransporter(int transporterId, int soId)
        {
            var so = new SO();

            return so;
        }

        /// <summary>
        /// Update specific SO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="soId"></param>
        /// <param name="so"></param>
        [HttpPut]
        [Route("transporters/{transporterId}/sos/{soId}")]
        public void PutSo(int transporterId, int soId, [FromBody]SO so)
        {

        }
    }
}
