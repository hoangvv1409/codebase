using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using ADR.Mobile.Infrastructure.Messaging.Sql.Handling;
using ADR.Mobile.Infrastructure.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging.Sql;

namespace Infrastructure.Messaging.Sql.IntegrationTests
{
    [TestClass]
    public class given_command_processor
    {
        private Mock<IMessageReceiver> receiverMock;
        private CommandProcessor processor;

        public given_command_processor()
        {
            this.receiverMock = new Mock<IMessageReceiver>();
            this.processor = new CommandProcessor(this.receiverMock.Object, CreateSerializer());
        }

        [TestMethod]
        public void when_starting_then_starts_receiver()
        {
            this.processor.Start();

            this.receiverMock.Verify(r => r.Start());
        }

        [TestMethod]
        public void when_stopping_after_starting_then_stops_receiver()
        {
            this.processor.Start();
            this.processor.Stop();

            this.receiverMock.Verify(r => r.Stop());
        }

        [TestMethod]
        public void when_receives_message_then_notifies_registered_handler()
        {
            var handlerAMock = new Mock<ICommandHandler>();
            handlerAMock.As<ICommandHandler<Command1>>();

            var handlerBMock = new Mock<ICommandHandler>();
            handlerBMock.As<ICommandHandler<Command2>>();

            this.processor.Register(handlerAMock.Object);
            this.processor.Register(handlerBMock.Object);

            this.processor.Start();

            var command1 = new Command1 { Id = Guid.NewGuid() };
            var command2 = new Command2 { Id = Guid.NewGuid() };

            this.receiverMock.Raise(r => r.MessageReceived += null, new MessageReceivedEventArgs(new Message(Serialize(command1))));
            this.receiverMock.Raise(r => r.MessageReceived += null, new MessageReceivedEventArgs(new Message(Serialize(command2))));

            handlerAMock.As<ICommandHandler<Command1>>().Verify(h => h.Handle(It.Is<Command1>(e => e.Id == command1.Id)));
            handlerBMock.As<ICommandHandler<Command2>>().Verify(h => h.Handle(It.Is<Command2>(e => e.Id == command2.Id)));
        }

        private static string Serialize(object payload)
        {
            var serializer = CreateSerializer();

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, payload);
                return writer.ToString();
            }
        }

        private static ITextSerializer CreateSerializer()
        {
            return new JsonTextSerializer();
        }

        public class Command1 : ICommand
        {
            public Guid Id { get; set; }
        }

        public class Command2 : ICommand
        {
            public Guid Id { get; set; }
        }
    }
}
