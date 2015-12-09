using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class POController : ApiController
    {
        /// <summary>
        /// Get all PO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/sos/pos")]
        public IEnumerable<PO> GetPosByTransporter(int transporterId)
        {
            var pos = new List<PO>();

            return pos;
        }

        /// <summary>
        /// Get all PO from specific Transporter and SO
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="soId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/sos/{soId}/pos")]
        public IEnumerable<PO> GetPoByTransporterAndSo(int transporterId, int soId)
        {
            var pos = new List<PO>();

            return pos;
        }

        /// <summary>
        /// Update all PO from specific Transporter and SO
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="soId"></param>
        /// <param name="po"></param>
        [HttpPut]
        [Route("transporter/{transporterId}/sos/{soId}/pos")]
        public void PutPos(int transporterId, int soId, [FromBody]IEnumerable<PO> po)
        {
            
        }
    }
}
