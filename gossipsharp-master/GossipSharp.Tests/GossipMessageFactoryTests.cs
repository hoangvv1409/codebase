using NUnit.Framework;
using ProtoBuf;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace GossipSharp.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class GossipMessageFactoryTests
    {
        private readonly int Timeout = Debugger.IsAttached ? 300000 : 1000;

        private const int NodePort1 = 30000;
        private const int NodePort2 = 30001;

        [ProtoContract]
        public class GossipMessage1 : GossipMessage
        {
            public GossipMessage1() : base(1) { }
        }

        [ProtoContract]
        public class GossipMessage2 : GossipMessage
        {
            public GossipMessage2() : base(2) { }
        }

        [Test]
        public void describe_custom_gossip_message_factory_creating_different_message_types()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort1));
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort2));
            var message1 = new GossipMessage1();
            var message2 = new GossipMessage2();
            GossipMessageFactory.CreateMessage += CreateMessage;

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            {
                var received = new AutoResetEvent(false);
                GossipMessage receivedMsg = null;
                node1.OnMessageReceived += (node, conn, msg) =>
                                               {
                                                   receivedMsg = msg;
                                                   received.Set();
                                               };

                node1.StartListening();
                node2.StartListening();

                node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));
                node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));

                node2.Cluster.BroadcastMessage(message1);
                if (!received.WaitOne(Timeout)) Assert.Fail();
                Assert.IsInstanceOf<GossipMessage1>(receivedMsg);

                node2.Cluster.BroadcastMessage(message2);
                if (!received.WaitOne(Timeout)) Assert.Fail();
                Assert.IsInstanceOf<GossipMessage2>(receivedMsg);
            }
        }

        private GossipMessage CreateMessage(int messageType, byte[] data)
        {
            switch (messageType)
            {
                case 1: return new GossipMessage1();
                case 2: return new GossipMessage2();
            }
            return new RawGossipMessage();
        }
    }
    // ReSharper restore InconsistentNaming
}
