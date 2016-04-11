using System;
using ADR.Mobile.Infrastructure.Messaging.Sql.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using ADR.Mobile.Infrastructure.Messaging.Sql;

namespace Infrastructure.Messaging.Sql.IntegrationTests
{
    [TestClass]
    public class given_sender : IDisposable
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly MessageSender sender;

        public given_sender()
        {
            this.connectionFactory = System.Data.Entity.Database.DefaultConnectionFactory;
            this.sender = new MessageSender(this.connectionFactory, "TestSqlMessaging", "Test.Commands");

            MessagingDbInitializer.CreateDatabaseObjects(this.connectionFactory.CreateConnection("TestSqlMessaging").ConnectionString, "Test", true);
        }

        void IDisposable.Dispose()
        {
            using (var connection = this.connectionFactory.CreateConnection("TestSqlMessaging"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "TRUNCATE TABLE Test.Commands";
                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void when_sending_string_message_then_saves_message()
        {
            var messageBody = "Message-" + Guid.NewGuid().ToString();
            var message = new Message(messageBody);

            this.sender.Send(message);

            //using (var context = this.contextFactory())
            //{
            //    Assert.True(context.Set<Message>().Any(m => m.Body.Contains(messageBody)));
            //}
        }
    }
}
