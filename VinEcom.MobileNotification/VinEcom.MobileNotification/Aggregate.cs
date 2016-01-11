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
        public Guid Id { get; protected set; }
        public IEvent Event { get; protected set; }

        protected Aggregate(Guid id)
        {
            this.Id = id;
        }

        public void GenerateEvent()
        {
            IEvent e = this.EventFactory();
            if (e != null)
            {
                this.Event = e;
            }
            else
            {
                throw new Exception("Not exist enum");
            }
        }

        protected abstract IEvent EventFactory();
    }
}
