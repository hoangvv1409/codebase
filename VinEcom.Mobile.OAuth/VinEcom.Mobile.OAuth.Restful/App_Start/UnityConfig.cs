using System;
using Microsoft.Practices.Unity;
using System.Web.Http;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus;
using ADR.Mobile.Infrastructure.Redis;
using ADR.Mobile.Infrastructure.Serialization;
using ADR.Mobile.Infrastructure.Settings;
using Unity.WebApi;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Database.Repository;
using VinEcom.Mobile.OAuth.Service;
using VinEcom.Mobile.OAuth.UserPublicClient;

namespace VinEcom.Mobile.OAuth.Restful
{
    public static class UnityConfig
    {
        public static void RegisterComponents(UnityContainer container)
        {
            #region Infrastructures

            string serviceBusSetting = AppDomain.CurrentDomain.BaseDirectory + "InfrastructureSetting.xml";

            MobileOAuthSettings infrastructureSetting = InfrastructureSettings.Read<MobileOAuthSettings>(serviceBusSetting);
            ServiceBusConfig serviceBusConfig = new ServiceBusConfig(infrastructureSetting.ServiceBus);
            RedisCacheSetting redisCacheConfig = infrastructureSetting.RedisCache;
            serviceBusConfig.Initialize();

            container.RegisterInstance<ITextSerializer>(new JsonTextSerializer());
            container.RegisterInstance<IMetadataProvider>(new StandardMetadataProvider());
            #endregion

            #region Command Bus
            container.RegisterInstance<IMessageSender>(Topics.Commands.Path, new TopicSender(infrastructureSetting.ServiceBus, Topics.Commands.Path));

            container.RegisterInstance<ICommandBus>(
                new CommandBus(
                    container.Resolve<IMessageSender>(Topics.Commands.Path),
                    container.Resolve<IMetadataProvider>(),
                    container.Resolve<ITextSerializer>()
                    ));
            #endregion

            #region Context
            container.RegisterType<OAuthDbContext>(
             new InjectionConstructor("MobileOAuth"));
            #endregion

            #region Cache Context
            container.RegisterType<RedisReadClient>(new ContainerControlledLifetimeManager(), new InjectionConstructor(redisCacheConfig));
            container.RegisterType<RedisWriteClient>(new ContainerControlledLifetimeManager(), new InjectionConstructor(redisCacheConfig));
            #endregion

            container.RegisterInstance<IUserClient>(new UserClient());

            container.RegisterType<IApplicationRepository, ApplicationRepository>();
            container.RegisterType<IAdminUserRepository, AdminUserRepository>();

            container.RegisterType<IAppService, ApplicationService>();
            container.RegisterType<IAdminUserService, AdminUserService>();
            container.RegisterType<IDeviceService, RedisDeviceService>();
            container.RegisterType<IRefreshTokenService, RedisRefreshTokenService>();
            container.RegisterType<IUserService, UserService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}