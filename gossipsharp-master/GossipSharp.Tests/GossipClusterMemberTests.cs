using System;
using System.Threading;
using NUnit.Framework;
using System.Net;
using System.Text;

namespace GossipSharp.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class GossipClusterMemberTests
    {
        private const int NodePort1 = 30000;
        private const int NodePort2 = 30001;

        [Test]
        public void describe_creating_connection_for_broadcasting_message_to_cluster()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort1));
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort2));
            var message = new RawGossipMessage();

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            {
                node1.StartListening();
                node2.StartListening();

                node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));
                node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));
                Assert.IsNotNull(node1.Cluster[nodeConfig2.NodeId]);
                Assert.IsNotNull(node2.Cluster[nodeConfig1.NodeId]);

                node1.Cluster.BroadcastMessage(message);
                Assert.AreEqual(1, node1.Cluster[nodeConfig2.NodeId].NumberOfOpenConnections);
                Assert.AreEqual(1, node2.Cluster[nodeConfig1.NodeId].NumberOfOpenConnections);
            }
        }

        [Test]
        public void describe_connecting_to_node_that_is_down()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort1));
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort2));
            var message = new RawGossipMessage();

            var connectionFailed = new ManualResetEventSlim(false);

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            {
                node1.StartListening();
                node2.StartListening();

                var wrongConfig = nodeConfig2.Clone();
                wrongConfig.BindToEndPoint = new IPEndPoint(wrongConfig.BindToEndPoint.Address, 1);
                var member = new GossipClusterMember(nodeConfig1.NodeId, wrongConfig, node2.Authenticator);
                member.ConnectionTimeout = TimeSpan.FromMilliseconds(300);
                member.OnConnectionFailed += (m, e) => connectionFailed.Set();

                node1.Cluster.Join(member);
                node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));

                node1.Cluster.BroadcastMessage(message);
                if (!connectionFailed.Wait(member.ConnectionTimeout.Add(TimeSpan.FromMilliseconds(100))))
                    Assert.Fail();
            }
        }

        [Test]
        public void describe_removal_of_expired_connections()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort1));
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort2));
            var message = new RawGossipMessage();

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            {
                node1.StartListening();
                node2.StartListening();

                node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));
                node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));

                node1.Cluster.BroadcastMessage(message);
                
                var clusterMember = node1.Cluster[nodeConfig2.NodeId];
                GossipClusterMember.MinConnectionInactivity = TimeSpan.Zero;
                clusterMember.MaxConnectionInactivity = TimeSpan.Zero;
                Thread.Sleep(1);
                clusterMember.RemoveInactiveConnections(null);
                Assert.AreEqual(0, clusterMember.NumberOfOpenConnections);
            }
        }

        [Test]
        public void describe_keeping_of_nonexpired_connections()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort1));
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort2));
            var message = new RawGossipMessage();

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            {
                node1.StartListening();
                node2.StartListening();

                node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));
                node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));

                node1.Cluster.BroadcastMessage(message);

                var clusterMember = node1.Cluster[nodeConfig2.NodeId];
                Thread.Sleep(1);
                clusterMember.RemoveInactiveConnections(null);
                Assert.AreEqual(1, clusterMember.NumberOfOpenConnections);
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
