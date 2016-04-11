using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging.Instrumentation
{
    public interface ISessionSubscriptionReceiverInstrumentation : ISubscriptionReceiverInstrumentation
    {
        void SessionStarted();

        void SessionEnded();
    }
}
