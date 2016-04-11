using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using ADR.Mobile.Infrastructure.Serialization;

namespace ADR.Mobile.Infrastructure.Messaging.Sql.Handling
{
    /// <summary>
    /// Processes incoming events from the bus and routes them to the appropriate 
    /// handlers.
    /// </summary>
    public class EventProcessor : MessageProcessor, IEventHandlerRegistry
    {
        private EventDispatcher messageDispatcher;

        public EventProcessor(IMessageReceiver receiver, ITextSerializer serializer)
            : base(receiver, serializer)
        {
            this.messageDispatcher = new EventDispatcher();
        }

        public void Register(IEventHandler eventHandler)
        {
            this.messageDispatcher.Register(eventHandler);
        }

        protected override void ProcessMessage(object payload, string correlationId)
        {
            var @event = (IEvent)payload;
            this.messageDispatcher.DispatchMessage(@event, null, correlationId, "");
        }
    }
}
