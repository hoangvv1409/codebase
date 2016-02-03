using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using VinEcom.User.PublicClient;

namespace JsonWebToken
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            string id, secret;
            if (context.TryGetFormCredentials(out id, out secret))
            {
                context.OwinContext.Set<string>("as:client_id", id);
                context.Validated();
                //if (secret == "secret")
                //{
                //    // need to make the client_id available for later security checks
                //    context.OwinContext.Set<string>("as:client_id", id);
                //    context.Validated();
                //}
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //var client = new UserClient();
            //var result = client.Login(request, out status, out message);
            //using (AuthRepository _repo = new AuthRepository())
            //{
            //    IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

            //    if (user == null)
            //    {
            //        context.SetError("invalid_grant", "The user name or password is incorrect.");
            //        return;
            //    }
            //}

            //create identity
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //var identity = new ClaimsIdentity("Embedded");
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            // create metadata to pass on to refresh token provider
            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                { "as:client_id", context.ClientId }
            });

            var ticket = new AuthenticationTicket(identity, props);

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
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
        }
    }

    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        //TODO Using Redis to store Dictionary. DB(Redis persistent) for persistence
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(5) //SET DATETIME to 5 Minutes
                //ExpiresUtc = DateTime.UtcNow.AddMonths(3) 
            };

            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            // maybe only create a handle the first time, then re-use
            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
        }
    }
}