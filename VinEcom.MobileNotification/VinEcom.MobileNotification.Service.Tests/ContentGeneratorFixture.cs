using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Service.Tests
{
    [TestClass]
    public class ContentGeneratorFixture
    {
        private ContentGenerator contentGenerator;
        private IEvent e;

        public ContentGeneratorFixture()
        {
            this.contentGenerator = new ContentGenerator();
            this.e = new OrderCancelled(Guid.NewGuid())
            {
                SOID = 123456,
                UserId = 123
            };
        }

        [TestMethod]
        public void when_given_an_event_and_template_then_generate_content1()
        {
            string template = "Order {SOID} has been cancelled";
            string content = contentGenerator.Generate(template, e);
            Assert.IsTrue(content == "Order 123456 has been cancelled");

            template = "Order {soid} has been cancelled";
            content = contentGenerator.Generate(template, e);
            Assert.IsTrue(content == "Order 123456 has been cancelled");

            template = "Order {soid} has been cancelled {SOID}";
            content = contentGenerator.Generate(template, e);
            Assert.IsTrue(content == "Order 123456 has been cancelled 123456");
        }

        [TestMethod]
        public void when_given_an_event_and_template_then_generate_content2()
        {
            string template = "To user {UserId}: Order {SOID} has been cancelled";
            string content = contentGenerator.Generate(template, e);

            Assert.IsTrue(content == "To user 123: Order 123456 has been cancelled");
        }

        [TestMethod]
        public void when_given_an_event_and_template_then_generate_content3()
        {
            string template = "Order has been cancelled";
            string content = contentGenerator.Generate(template, e);

            Assert.IsTrue(content == "Order has been cancelled");
        }

        [TestMethod]
        public void when_given_an_event_and_template_then_generate_content4()
        {
            string template = "Shipment {ShipmentId} has been cancelled";
            string content = contentGenerator.Generate(template, e);

            Assert.IsTrue(content == "Shipment {ShipmentId} has been cancelled");
        }
    }
}
