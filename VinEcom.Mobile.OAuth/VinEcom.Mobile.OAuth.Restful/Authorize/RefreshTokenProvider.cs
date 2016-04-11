using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ADR.Mobile.Infrastructure.Messaging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Serializer;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Contracts.Commands;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.Commands;

namespace VinEcom.Mobile.OAuth.Restful.Authorize
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private IRefreshTokenService refreshTokenService;
        private ICommandBus commandBus;
        private int expriredTime;
        private TicketSerializer serializer;
        private int ttlChange;

        public RefreshTokenProvider(IRefreshTokenService refreshTokenService, ICommandBus commandBus, int refreshTokenExpiredTimeInMinutes, int ttlChange)
        {
            this.refreshTokenService = refreshTokenService;
            this.commandBus = commandBus;
            this.expriredTime = refreshTokenExpiredTimeInMinutes;
            this.ttlChange = ttlChange;
            this.serializer = new TicketSerializer();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            if (context.Ticket.Properties.Dictionary["as:type"] != "admin")
            {
                var refreshToken = CreateRefreshTokenEntity(context);
                var command = CreateCommand(refreshToken);

                context.SetToken(refreshToken.RefreshTokenValue.ToString());
                //TODO Do we need persist before Send CreateRefreshToken, CreateUserDevice then delete?
                this.commandBus.Send(command);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string clientId = context.OwinContext.Get<string>("as:client_id");
            string ticket = refreshTokenService.GetAuthenticationTicket(new Guid(context.Token), clientId);
            if (!string.IsNullOrEmpty(ticket) && !string.IsNullOrWhiteSpace(ticket))
            {
                context.SetTicket(serializer.Deserialize(System.Text.Encoding.Default.GetBytes(ticket)));

                var command = new ReduceRefreshTokenTTL()
                {
                    ClientId = clientId,
                    RefreshToken = Guid.Parse(context.Token),
                    AuthenticationTicket = ticket,
                    TTL = ttlChange
                };

                this.commandBus.Send(command);
            }
        }

        private RefreshToken CreateRefreshTokenEntity(AuthenticationTokenCreateContext context)
        {
            var guid =
                new Guid(
                    context.Ticket.Identity.Claims.Where(c => c.Type == "RefreshToken")
                        .Select(c => c.Value)
                        .FirstOrDefault());
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];
            var refreshToken = new RefreshToken(guid, clientId, this.expriredTime);

            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = refreshToken.CreatedTime,
                ExpiresUtc = refreshToken.ExpiredTime
            });

            refreshToken.SetAuthenticationTicket(System.Text.Encoding.Default.GetString(serializer.Serialize(refreshTokenTicket)));

            return refreshToken;
        }

        private CreateRefreshToken CreateCommand(RefreshToken refreshToken)
        {
            return new CreateRefreshToken()
            {
                RefreshToken = refreshToken.RefreshTokenValue,
                AuthenticationTicket = refreshToken.AuthenticationTicket,
                ClientId = refreshToken.ClientId,
                CreatedTime = refreshToken.CreatedTime,
                ExpiredTime = refreshToken.ExpiredTime
            };
        }

        #region NotImplement
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}