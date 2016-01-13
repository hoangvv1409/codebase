using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;
using VinEcom.MobileNotification.Service;
using VinEcom.MobileNotification.Service.Handlers;

namespace VinEcom.MobileNotification.ConsoleWorker
{
    public class Container
    {
        public static IUnityContainer CreateContainer()
        {
            UnityContainer container = new UnityContainer();

            #region Infrastructure
            InfrastructureSettings infrastructureSetting = InfrastructureSettings.Read("E:/ADR/Mobile/VinEcom.MobileNotification/VinEcom.MobileNotification.InternalApi/InfrastructureSetting.xml");
            ServiceBusConfig serviceBusConfig = new ServiceBusConfig(infrastructureSetting.ServiceBus);
            serviceBusConfig.Initialize();

            container.RegisterInstance<ITextSerializer>(new JsonTextSerializer());
            container.RegisterInstance<IMetadataProvider>(new StandardMetadataProvider());
            #endregion

            #region Event Bus
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

            #region Handler Register

            container.RegisterType<IContentGenerator, ContentGenerator>();
            container.RegisterEventProcessor<OrderEventHandler>(serviceBusConfig, Topics.Events.Subscriptions.Order, false);
            container.RegisterEventProcessor<ShipmentEventHandler>(serviceBusConfig, Topics.Events.Subscriptions.Shipment, false);
            container.RegisterEventProcessor<UserEventHandler>(serviceBusConfig, Topics.Events.Subscriptions.User, false);
            #endregion

            return container;
        }
    }

    class ConstantValue
    {
        /// <summary>
        /// instance name for event sender
        /// </summary>
        public const string EventMessageSenderName = "transportation/eventsender";
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
            }
        }
    }
}
