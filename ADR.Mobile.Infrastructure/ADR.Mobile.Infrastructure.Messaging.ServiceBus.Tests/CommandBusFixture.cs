using System;
using System.Linq;
using ADR.Mobile.Infrastructure.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Messaging.ServiceBus;

namespace Infrastructure.Messaging.ServiceBus.Tests
{
    [TestClass]
    public class CommandBusFixture
    {
        [TestMethod]
        public void when_sending_then_sets_command_id_as_messageid()
        {
            var sender = new MessageSenderMock();
            var sut = new CommandBus(sender, Mock.Of<IMetadataProvider>(), new JsonTextSerializer());

            var command = new FooCommand { Id = Guid.NewGuid() };
            sut.Send(command);

            Assert.IsTrue(command.Id.ToString() == sender.Sent.Single().MessageId);
        }

        [TestMethod]
        public void when_specifying_time_to_live_then_sets_in_message()
        {
            var sender = new MessageSenderMock();
            var sut = new CommandBus(sender, Mock.Of<IMetadataProvider>(), new JsonTextSerializer());

            var command = new Envelope<ICommand>(new FooCommand { Id = Guid.NewGuid() })
            {
                TimeToLive = TimeSpan.FromMinutes(15)
            };
            sut.Send(command);

            Assert.IsTrue(sender.Sent.Single().TimeToLive > TimeSpan.FromMinutes(14.9) && sender.Sent.Single().TimeToLive < TimeSpan.FromMinutes(15.1));
        }

        [TestMethod]
        public void when_specifying_delay_then_sets_in_message()
        {
            var sender = new MessageSenderMock();
            var sut = new CommandBus(sender, Mock.Of<IMetadataProvider>(), new JsonTextSerializer());

            var command = new Envelope<ICommand>(new FooCommand { Id = Guid.NewGuid() })
            {
                Delay = TimeSpan.FromMinutes(15)
            };
            sut.Send(command);
            Assert.IsTrue(sender.Sent.Single().TimeToLive > TimeSpan.FromMinutes(14.9) && sender.Sent.Single().TimeToLive < TimeSpan.FromMinutes(15.1));
        }

        class FooCommand : ICommand
        {
            public Guid Id { get; set; }
        }
    }
}
