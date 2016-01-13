using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using VinEcom.MobileNotification.Infrastructure;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.ConsoleWorker
{
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
