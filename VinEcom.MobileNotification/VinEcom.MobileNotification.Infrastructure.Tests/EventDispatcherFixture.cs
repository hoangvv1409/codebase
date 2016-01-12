using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using Moq;

namespace VinEcom.MobileNotification.Infrastructure.Tests
{
    [TestClass]
    public class given_an_empty_dispatcher
    {
        private EventDispatcher dispatcher;

        public given_an_empty_dispatcher()
        {
            this.dispatcher = new EventDispatcher();
        }

        [TestMethod]
        public void when_dispatching_an_event_then_does_nothing()
        {
            var @event = new EventA();

            this.dispatcher.DispatchMessage(@event, "message", "correlationId", "");
        }
    }

    [TestClass]
    public class given_dispatcher_with_handler
    {
        private EventDispatcher dispatcher;
        private Mock<IEventHandler> handlerMock;

        public given_dispatcher_with_handler()
        {
            this.dispatcher = new EventDispatcher();

            this.handlerMock = new Mock<IEventHandler>();
            //this.handlerMock.As<IEventHandler<EventA>>();
            this.handlerMock.As<IEnvelopedEventHandler<EventA>>();

            this.dispatcher.Register(this.handlerMock.Object);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_registered_handler_then_invokes_handler()
        {
            var @event = new EventA();
            Guid id = Guid.NewGuid();
            this.dispatcher.DispatchMessage(@event, "msg", "correlation", "");

            //this.handlerMock.As<IEventHandler<EventA>>().Verify(h => h.Handle(It.Is<EventA>(e => e.Id == id)), Times.Once);

            this.handlerMock.As<IEnvelopedEventHandler<EventA>>().Verify(h => h.Handle(It.Is<Envelope<EventA>>(e => e.MessageId == "msg" && e.Body == @event && e.CorrelationId == "correlation")), Times.Once);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_no_registered_handler_then_doeas_nothing()
        {

        }
    }

    public class EventA : IEvent
    {
        public Guid Id
        {
            get { return Guid.Empty; }
        }
    }

    public class EventB : IEvent
    {
        public Guid Id
        {
            get { return Guid.Empty; }
        }
    }

    public class EventC : IEvent
    {
        public Guid Id
        {
            get { return Guid.Empty; }
        }
    }
}
