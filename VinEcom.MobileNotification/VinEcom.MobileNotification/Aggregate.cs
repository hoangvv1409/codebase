using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public abstract class Aggregate
    {
        public IEvent Event { get; protected set; }

        public void GenerateEvent()
        {
            IEvent @event = this.EventFactory();
            if (@event != null)
            {
                this.Event = @event;
            }
            else
            {
                throw new Exception("Not exist enum");
            }
        }

        protected abstract IEvent EventFactory();
    }
}
