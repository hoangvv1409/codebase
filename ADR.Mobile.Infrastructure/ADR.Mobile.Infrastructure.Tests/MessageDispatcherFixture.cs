using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Infrastructure.Tests
{
    [TestClass]
    public class given_empty_dispatcher
    {
        private EventDispatcher sut;

        public given_empty_dispatcher()
        {
            this.sut = new EventDispatcher();
        }

        [TestMethod]
        public void when_dispatching_an_event_then_does_nothing()
        {
            var @event = new EventC();

            this.sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    [TestClass]
    public class given_dispatcher_with_handler
    {
        private EventDispatcher sut;
        private Mock<IEventHandler> handlerMock;

        public given_dispatcher_with_handler()
        {
            this.sut = new EventDispatcher();

            this.handlerMock = new Mock<IEventHandler>();
            this.handlerMock.As<IEventHandler<EventA>>();

            this.sut.Register(this.handlerMock.Object);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_registered_handler_then_invokes_handler()
        {
            var @event = new EventA();

            this.sut.DispatchMessage(@event, "message", "correlation", "");

            this.handlerMock.As<IEventHandler<EventA>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [TestMethod]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            this.sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    [TestClass]
    public class given_dispatcher_with_handler_for_envelope
    {
        private EventDispatcher sut;
        private Mock<IEventHandler> handlerMock;

        public given_dispatcher_with_handler_for_envelope()
        {
            this.sut = new EventDispatcher();

            this.handlerMock = new Mock<IEventHandler>();
            this.handlerMock.As<IEnvelopedEventHandler<EventA>>();

            this.sut.Register(this.handlerMock.Object);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_registered_handler_then_invokes_handler()
        {
            var @event = new EventA();

            this.sut.DispatchMessage(@event, "message", "correlation", "");

            this.handlerMock.As<IEnvelopedEventHandler<EventA>>()
                .Verify(
                    h => h.Handle(It.Is<Envelope<EventA>>(e => e.Body == @event && e.MessageId == "message" && e.CorrelationId == "correlation")),
                    Times.Once());
        }

        [TestMethod]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            this.sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    [TestClass]
    public class given_dispatcher_with_multiple_handlers
    {
        private EventDispatcher sut;
        private Mock<IEventHandler> handler1Mock;
        private Mock<IEventHandler> handler2Mock;

        public given_dispatcher_with_multiple_handlers()
        {
            this.sut = new EventDispatcher();

            this.handler1Mock = new Mock<IEventHandler>();
            this.handler1Mock.As<IEnvelopedEventHandler<EventA>>();
            this.handler1Mock.As<IEventHandler<EventB>>();

            this.sut.Register(this.handler1Mock.Object);

            this.handler2Mock = new Mock<IEventHandler>();
            this.handler2Mock.As<IEventHandler<EventA>>();

            this.sut.Register(this.handler2Mock.Object);
        }

        [TestMethod]
        public void when_dispatching_an_event_with_multiple_registered_handlers_then_invokes_handlers()
        {
            var @event = new EventA();

            this.sut.DispatchMessage(@event, "message", "correlation", "");

            this.handler1Mock.As<IEnvelopedEventHandler<EventA>>()
                .Verify(
                    h => h.Handle(It.Is<Envelope<EventA>>(e => e.Body == @event && e.MessageId == "message" && e.CorrelationId == "correlation")),
                    Times.Once());
            this.handler2Mock.As<IEventHandler<EventA>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [TestMethod]
        public void when_dispatching_an_event_with_single_registered_handler_then_invokes_handler()
        {
            var @event = new EventB();

            this.sut.DispatchMessage(@event, "message", "correlation", "");

            this.handler1Mock.As<IEventHandler<EventB>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [TestMethod]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            this.sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    public class EventA : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }

    public class EventB : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }

    public class EventC : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }
}
