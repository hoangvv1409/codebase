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
            var @event = new EventC();

            this.dispatcher.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    [TestClass]
    public class given_dispatcher_with_multiple_handlers
    {
        private EventDispatcher dispatcher;
        private Mock<IEventHandler> handler1Mock;
        private Mock<IEventHandler> handler2Mock;

        public given_dispatcher_with_multiple_handlers()
        {
            this.dispatcher = new EventDispatcher();
            this.handler1Mock = new Mock<IEventHandler>();
            this.handler2Mock = new Mock<IEventHandler>();

            this.handler1Mock.As<IEnvelopedEventHandler<EventA>>();
            this.handler1Mock.As<IEventHandler<EventB>>(); this.handler2Mock.As<IEventHandler<EventA>>();

            this.dispatcher.Register(this.handler1Mock.Object);
            this.dispatcher.Register(this.handler2Mock.Object);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_multiple_registered_handlers_then_invokes_handlers()
        {
            var @event = new EventA();

            this.dispatcher.DispatchMessage(@event, "message", "correlation", "");

            this.handler1Mock.As<IEnvelopedEventHandler<EventA>>()
                .Verify(h => h.Handle(It.Is<Envelope<EventA>>(e => e.Body == @event && e.MessageId == "message" && e.CorrelationId == "correlation")),
                    Times.Once());

            this.handler2Mock.As<IEventHandler<EventA>>().Verify(h => h.Handle(@event), Times.Once);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_single_registered_handler_then_invokes_handler()
        {
            var @event = new EventB();

            this.dispatcher.DispatchMessage(@event, "message", "correlation", "");

            this.handler1Mock.As<IEventHandler<EventB>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [TestMethod]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            this.dispatcher.DispatchMessage(@event, "message", "correlation", "");
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
