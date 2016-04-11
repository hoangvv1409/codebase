using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus.Instrumentation
{
    public static class Constants
    {
        public const string ReceiversPerformanceCountersCategory = "Window ServiceBus Infrastructure - Receivers";
        public const string EventPublishersPerformanceCountersCategory = "Window ServiceBus Infrastructure - Event Publishers";
    }
}
