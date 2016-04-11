using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using ADR.Mobile.Infrastructure.Messaging;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using Owin;
using Unity.WebApi;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Restful.Authorize;

[assembly: OwinStartup(typeof(VinEcom.Mobile.OAuth.Restful.Startup))]

namespace VinEcom.Mobile.OAuth.Restful
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            var container = new UnityContainer();

            UnityConfig.RegisterComponents(container);
            ConfigureOAuth(app, container);

            config.DependencyResolver = new UnityDependencyResolver(container); ;
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app, UnityContainer container)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenExpired"])),
                Provider = new AuthorizationServerProvider(container.Resolve<IUserService>(), container.Resolve<IAdminUserService>(), container.Resolve<IAppService>(), container.Resolve<ICommandBus>()),
                RefreshTokenProvider = new RefreshTokenProvider(container.Resolve<IRefreshTokenService>(), container.Resolve<ICommandBus>(), Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpired"]), Convert.ToInt32(ConfigurationManager.AppSettings["TTLChange"]))
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
