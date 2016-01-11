﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging.Handling
{
    public class EventProcessor : MessageProcessor, IEventHandlerRegistry
    {
        private EventDispatcher eventDispatcher;

        public EventProcessor(IMessageReceiver receiver, ITextSerializer textSerializer)
            : base(receiver, textSerializer)
        {
            this.eventDispatcher = new EventDispatcher();
        }

        public void Register(IEventHandler handler)
        {
            this.eventDispatcher.Register(handler);
        }

        protected override void ProcessMessage(string traceIdentifier, object payload, string messageId, string correlationId, BrokerdMessageInformation messageInformation)
        {
            var e = payload as IEvent;
            if (e != null)
            {
                this.eventDispatcher.DispatchMessage(e, messageId, correlationId, traceIdentifier);
            }
        }
    }
}