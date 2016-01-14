using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Service;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(VinEcom.MobileNotification.InternalApi.App_Start.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(VinEcom.MobileNotification.InternalApi.App_Start.UnityWebActivator), "Shutdown")]

namespace VinEcom.MobileNotification.InternalApi.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            #region Infrastructures
            string serviceBusSetting = ConfigurationManager.AppSettings["ServiceBusSetting"];
            InfrastructureSettings infrastructureSetting = InfrastructureSettings.Read(serviceBusSetting);
            ServiceBusConfig serviceBusConfig = new ServiceBusConfig(infrastructureSetting.ServiceBus);
            serviceBusConfig.Initialize();

            container.RegisterInstance<ITextSerializer>(new JsonTextSerializer());
            container.RegisterInstance<IMetadataProvider>(new StandardMetadataProvider());
            #endregion

            #region Event Bus
            // event bus
            container.RegisterInstance<IMessageSender>(ConstantValue.EventMessageSenderName, new TopicSender(infrastructureSetting.ServiceBus, Topics.Events.Path));
            container.RegisterInstance<IEventBus>(
                new EventBus(
                    container.Resolve<IMessageSender>(ConstantValue.EventMessageSenderName),
                    container.Resolve<ITextSerializer>(),
                    container.Resolve<IMetadataProvider>()));
            #endregion

            #region Context
            container.RegisterType<MobileNotificationDbContext>(
             new InjectionConstructor("MobileNotification"));
            #endregion

            #region Service
            container.RegisterType<NotificationService>();
            #endregion

            //DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            // Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }

    class ConstantValue
    {
        /// <summary>
        /// instance name for event sender
        /// </summary>
        public const string EventMessageSenderName = "notification/events";
        public const string MobileNotificationDbContext = "MobileNotificationDbContext";
        public const string NotificationService = "NotificationService";
    }

    public static class Topics
    {
        public static class Events
        {
            public const string Path = "notification/events";

            public static class Subscriptions
            {
                public const string Order = "Order";
                public const string Shipment = "Shipment";
                public const string User = "User";
                public const string PushNotification = "PushNotification";
            }
        }
    }
}