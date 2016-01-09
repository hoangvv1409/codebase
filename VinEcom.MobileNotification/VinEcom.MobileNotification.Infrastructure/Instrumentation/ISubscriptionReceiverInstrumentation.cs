using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Instrumentation
{
    public interface ISubscriptionReceiverInstrumentation
    {
        void MessageReceived();

        void MessageProcessed(bool success, long elapsedMilliseconds);

        void MessageCompleted(bool success);
    }
}
