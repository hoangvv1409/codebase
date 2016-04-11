using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Restful.Models;

namespace VinEcom.Mobile.OAuth.Restful.Controllers
{
    public class HealthCheckController : ApiController
    {
        // GET: api/HealthCheck
        public HealthCheckModel Get()
        {
            bool dbStatus = true;
            try
            {
                OAuthDbContext dbContext = new OAuthDbContext("MobileOAuth");
                dbContext.Database.Connection.Open();
                dbContext.Database.Connection.Close();
            }
            catch (SqlException)
            {
                dbStatus = false;
            }

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            HealthCheckModel healthCheckModel = new HealthCheckModel()
            {
                Version = version.ToString(),
                ConnectDBStatus = dbStatus
            };

            return healthCheckModel;
        }
    }
}
