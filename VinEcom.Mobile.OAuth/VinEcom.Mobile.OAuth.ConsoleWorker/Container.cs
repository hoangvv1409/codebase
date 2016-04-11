using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus;
using ADR.Mobile.Infrastructure.Redis;
using ADR.Mobile.Infrastructure.Serialization;
using ADR.Mobile.Infrastructure.Settings;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Service;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus.Handling;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus.Instrumentation;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Service.Handlers;
using VinEcom.Mobile.OAuth.Database.Repository;

namespace VinEcom.Mobile.OAuth.ConsoleWorker
{
    public class Container
    {
        public static IUnityContainer CreateContainer()
        {
            UnityContainer container = new UnityContainer();

            #region Infrastructure

            string serviceBusSetting = AppDomain.CurrentDomain.BaseDirectory + "InfrastructureSetting.xml";
            MobileOAuthSettings infrastructureSetting = InfrastructureSettings.Read<MobileOAuthSettings>(serviceBusSetting);
            ServiceBusConfig serviceBusConfig = new ServiceBusConfig(infrastructureSetting.ServiceBus);
            RedisCacheSetting redisCacheConfig = infrastructureSetting.RedisCache;
            serviceBusConfig.Initialize();

            container.RegisterInstance<ITextSerializer>(new JsonTextSerializer());
            container.RegisterInstance<IMetadataProvider>(new StandardMetadataProvider());
            #endregion

            #region Command Bus
            // event bus
            container.RegisterInstance<IMessageSender>(Topics.Commands.Path, new TopicSender(infrastructureSetting.ServiceBus, Topics.Commands.Path));
            container.RegisterInstance<ICommandBus>(
                new CommandBus(
                    container.Resolve<IMessageSender>(Topics.Commands.Path),
                    container.Resolve<IMetadataProvider>(),
                    container.Resolve<ITextSerializer>()
                    ));
            #endregion

            #region Event Bus
            container.RegisterInstance<IMessageSender>(Topics.Events.Path, new TopicSender(infrastructureSetting.ServiceBus, Topics.Events.Path));
            container.RegisterInstance<IEventBus>(
                new EventBus(
                    container.Resolve<IMessageSender>(Topics.Events.Path),
                    container.Resolve<IMetadataProvider>(),
                    container.Resolve<ITextSerializer>()));
            #endregion

            #region Cache Context
            container.RegisterType<RedisReadClient>(new ContainerControlledLifetimeManager(), new InjectionConstructor(redisCacheConfig));
            container.RegisterType<RedisWriteClient>(new ContainerControlledLifetimeManager(), new InjectionConstructor(redisCacheConfig));
            #endregion

            #region Context
            container.RegisterType<OAuthDbContext>(
             new InjectionConstructor("MobileOAuth"));
            #endregion

            #region Repository
            container.RegisterType<IUserDeviceRepository, UserDeviceRepository>();
            #endregion

            #region CommandHandler
            container.RegisterType<ICommandHandler, OAuthViewGenerator>("OAuthViewGenerator");

            var oAuthViewGenerator =
                new CommandProcessor(new SessionSubscriptionReceiver(infrastructureSetting.ServiceBus, Topics.Commands.Path, Topics.Commands.Subscriptions.OAuthViewGenerator, false, new SessionSubscriptionReceiverInstrumentation(Topics.Commands.Subscriptions.OAuthViewGenerator, false)), container.Resolve<ITextSerializer>());

            oAuthViewGenerator.Register(container.Resolve<ICommandHandler>("OAuthViewGenerator"));

            container.RegisterInstance<IProcessor>("OAuthViewGeneratorProcessor", oAuthViewGenerator);
            #endregion

            container.RegisterEventProcessor<AuthenInfoGenerator>(serviceBusConfig, Topics.Events.Subscriptions.AuthenInfoSync, false);

            return container;
        }
    }

    public static class UnityContainerExtensions
    {
        public static void RegisterEventProcessor<T>(this IUnityContainer container, ServiceBusConfig busConfig, string subscriptionName, bool instrumentationEnabled = false)
           where T : IEventHandler
        {
            container.RegisterInstance<IProcessor>(subscriptionName, busConfig.CreateEventProcessor(
                subscriptionName,
                container.Resolve<T>(),
                container.Resolve<ITextSerializer>(),
                instrumentationEnabled));
        }
    }
}
