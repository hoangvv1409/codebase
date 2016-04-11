using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging;
using Microsoft.Owin.Security;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.Commands;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;

namespace VinEcom.Mobile.OAuth.Restful.Authorize
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private IUserService userService;
        private IAdminUserService adminUserService;
        private IAppService appService;
        private ICommandBus commandBus;

        public AuthorizationServerProvider(IUserService userService, IAdminUserService adminUserService,
            IAppService appService, ICommandBus commandBus)
        {
            this.userService = userService;
            this.adminUserService = adminUserService;
            this.appService = appService;
            this.commandBus = commandBus;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //TODO Validate null property
            string id, secret;
            context.TryGetFormCredentials(out id, out secret);

            var type = context.Parameters.Get("type");
            switch (type)
            {
                case "admin":
                    if (id == null) id = context.Parameters.Get("Username") + "_SysAdmin";
                    context.Validated();
                    break;
                case "app":
                    if (secret != null) context.Validated();
                    break;
                default:
                    if (id != null) context.Validated();
                    type = string.Empty;
                    break;
            }

            context.OwinContext.Set<string>("as:client_id", id);
            context.OwinContext.Set<string>("as:client_secret", secret);
            context.OwinContext.Set<string>("as:type", type);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            IEnumerable<ClaimModel> claims = GetClaimModels(context);

            if (claims == null) return;

            string clientId = context.ClientId ?? context.OwinContext.Get<string>("as:client_id");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaims(claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)));
            identity.AddClaim(new Claim("ClientId", clientId));
            identity.AddClaim(new Claim("RefreshToken", Guid.NewGuid().ToString()));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"as:client_id", clientId},
                {"as:type", context.OwinContext.Get<string>("as:type")}
            });

            var ticket = new AuthenticationTicket(identity, props);

            if (context.OwinContext.Get<string>("as:type") == string.Empty)
            {
                var createUserDevice = new CreateUserDevice()
                {
                    ClientId = clientId,
                    UserId = Convert.ToInt32(identity.Claims.Where(c => c.Type == "UserId")
                        .Select(c => c.Value)
                        .FirstOrDefault())
                };

                this.commandBus.Send(createUserDevice);
            }

            context.Validated(ticket);
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.OwinContext.Get<string>("as:client_id");

            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
                return;
            }

            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);

            newId.RemoveClaim(newId.FindFirst(c => c.Type == "RefreshToken"));
            newId.AddClaim(new Claim("RefreshToken", Guid.NewGuid().ToString()));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
        }

        private IEnumerable<ClaimModel> GetClaimModels(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string message = "";
            var type = context.OwinContext.Get<string>("as:type");
            IEnumerable<ClaimModel> claimModels = null;

            switch (type)
            {
                case "admin":
                    if (this.adminUserService.IsExist(context.UserName, context.Password))
                        claimModels = this.adminUserService.GetAdminUserClaims(context.UserName);
                    break;
                case "app":
                    var appId = Guid.Parse(context.ClientId);
                    if (this.appService.IsExist(appId, context.OwinContext.Get<string>("as:client_secret")))
                        claimModels = this.appService.GetApplicationClaims(appId);
                    break;
                default:
                    claimModels = this.userService.Login(context.UserName, context.Password, out message);
                    break;
            }

            if (claimModels == null)
            {
                context.SetError("invalid_grant", message);
            }

            return claimModels;
        }
    }
}