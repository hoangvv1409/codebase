using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace GossipSharp.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class GossipMessageTests
    {
        [Test]
        public void serialize_and_deserialize_all_message_types()
        {
            int numberOfTypesTested = 0;

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a != Assembly.GetExecutingAssembly())
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(GossipMessage))))
            {
                var message = (GossipMessage)Activator.CreateInstance(type);
                var serializedBuffer = message.Serialize();
                var deserializeMethod = typeof(GossipMessage).GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
                var deserializedMessage = deserializeMethod.Invoke(null, new object[] { serializedBuffer });
                Assert.NotNull(deserializedMessage);
                numberOfTypesTested++;
            }

            Assert.AreNotEqual(0, numberOfTypesTested);
        }
    }
    // ReSharper restore InconsistentNaming
}
