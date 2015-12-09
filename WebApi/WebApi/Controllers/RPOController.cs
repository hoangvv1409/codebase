using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class RPOController : ApiController
    {
        /// <summary>
        /// Get all RPO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/rpos")]
        public IEnumerable<RPO> GetRposByTransporter(int transporterId)
        {
            var rpos = new List<RPO>();

            return rpos;
        }

        /// <summary>
        /// Get specific RPO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="rpoId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("transporters/{transporterId}/rpos/{rpoId}")]
        public RPO GetSoByTransporter(int transporterId, int rpoId)
        {
            var rpo = new RPO();

            return rpo;
        }

        /// <summary>
        /// Update specific RPO from specific Transporter
        /// </summary>
        /// <param name="transporterId"></param>
        /// <param name="rpoId"></param>
        /// <param name="so"></param>
        [HttpPut]
        [Route("transporters/{transporterId}/rpos/{rpoId}")]
        public void PutRpo(int transporterId, int rpoId, [FromBody]RPO so)
        {

        }
    }
}
