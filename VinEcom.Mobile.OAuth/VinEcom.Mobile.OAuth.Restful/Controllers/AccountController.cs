using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Web.Http;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Redis;
using VinEcom.Mobile.Authen.Contracts.Events;
using VinEcom.Mobile.OAuth.Contracts.Commands;
using VinEcom.Mobile.OAuth.Core.Commands;

namespace VinEcom.Mobile.OAuth.Restful.Controllers
{
    public class AccountController : ApiController
    {
        private ICommandBus commandBus;
        private RedisReadClient redisReadClient;
        private string UserDevicePrefixNamespace;
        private string RefreshTokenPrefixNamespace;

        public AccountController(ICommandBus commandBus, RedisReadClient redisReadClient)
        {
            this.commandBus = commandBus;
            this.redisReadClient = redisReadClient;

            this.UserDevicePrefixNamespace = "UserDevice";
            this.RefreshTokenPrefixNamespace = "Ticket";
        }

        [HttpGet]
        [Route("ChangePassword/{userId}/{clientId}")]
        public HttpResponseMessage ChangePassword(long userId, string clientId)
        {
            var command = new ForceUserLogin()
            {
                UserId = userId,
                ClientId = clientId
            };
            this.commandBus.Send(command);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("GetUserDevice/{userId}/{clientId}")]
        public HttpResponseMessage GetUserDevice(long userId, string clientId)
        {
            string key = string.Format("{0}:{1}:{2}", this.UserDevicePrefixNamespace, userId, clientId);
            string json = this.redisReadClient.Get(key);

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        }

        [Authorize]
        [HttpGet]
        [Route("logout")]
        public HttpResponseMessage LogOut()
        {
            try
            {
                var identity = (User.Identity as ClaimsIdentity);
                var clientId = identity.Claims.Where(c => c.Type == "ClientId").Select(c => c.Value).FirstOrDefault();
                var refreshToken =
                    identity.Claims.Where(c => c.Type == "RefreshToken").Select(c => c.Value).FirstOrDefault();

                var command = new RemoveRefreshToken()
                {
                    ClientId = clientId,
                    RefreshToken = Guid.Parse(refreshToken)
                };

                this.commandBus.Send(command);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("RemoveToken/{clientId}/{refreshToken}")]
        public HttpResponseMessage RemoveToken(string clientId, string refreshToken)
        {
            try
            {
                var command = new RemoveRefreshToken()
                {
                    ClientId = clientId,
                    RefreshToken = Guid.Parse(refreshToken)
                };

                this.commandBus.Send(command);
            }
            catch { }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
