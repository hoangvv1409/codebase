using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Instrumentation;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus.Instrumentation
{
    public class SessionSubscriptionReceiverInstrumentation : SubscriptionReceiverInstrumentation, ISessionSubscriptionReceiverInstrumentation
    {
        public const string TotalSessionsCounterName = "Total sessions";
        public const string CurrentSessionsCounterName = "Current sessions";

        private readonly PerformanceCounter totalSessionsCounter;
        private readonly PerformanceCounter currentSessionsCounter;

        public SessionSubscriptionReceiverInstrumentation(string instanceName, bool instrumentationEnabled)
            : base(instanceName, instrumentationEnabled)
        {
            if (this.InstrumentationEnabled)
            {
                this.totalSessionsCounter = new PerformanceCounter(Constants.ReceiversPerformanceCountersCategory, TotalSessionsCounterName, this.InstanceName, false);
                this.currentSessionsCounter = new PerformanceCounter(Constants.ReceiversPerformanceCountersCategory, CurrentSessionsCounterName, this.InstanceName, false);

                this.totalSessionsCounter.RawValue = 0;
                this.currentSessionsCounter.RawValue = 0;
            }
        }

        public void SessionStarted()
        {
            if (this.InstrumentationEnabled)
            {
                try
                {
                    this.totalSessionsCounter.Increment();
                    this.currentSessionsCounter.Increment();
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        public void SessionEnded()
        {
            if (this.InstrumentationEnabled)
            {
                try
                {
                    this.currentSessionsCounter.Decrement();
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.InstrumentationEnabled)
                {
                    this.totalSessionsCounter.Dispose();
                    this.currentSessionsCounter.Dispose();
                }
            }
        }
    }
}
