using NUnit.Framework;
using System.Net;
using System.Text;
using System.Threading;

namespace GossipSharp.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class GossipConnectionTests
    {
        private const int NodePort = 30000;

        [Test]
        public void describe_connection_with_successful_authentication()
        {
            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort));

            using (var node = new GossipNode(nodeConfig, clusterConfig))
            {
                var signal = new ManualResetEventSlim(false);
                bool? authenticated = null;
                node.OnClientConnectionAuthenticationFailed += (n, c) => { authenticated = false; signal.Set(); };
                node.OnClientConnectionAuthenticationSucceeded += (n, c) => { authenticated = true; signal.Set(); };
                node.StartListening();
                using (GossipConnection.ConnectAsync(nodeConfig.NodeId, nodeConfig.BindToEndPoint, node.Authenticator, c => signal.Set()))
                {
                    signal.Wait();
                    Assert.That(authenticated == true);
                }
            }
        }

        [Test]
        public void describe_connection_with_failed_authentication_due_to_incorrect_clusterkey()
        {
            var clusterConfigNode = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig = new GossipNodeConfig(new IPEndPoint(IPAddress.Loopback, NodePort));

            using (var node = new GossipNode(nodeConfig, clusterConfigNode))
            {
                var signal = new ManualResetEventSlim(false);
                bool? authenticated = null;
                node.OnClientConnectionAuthenticationFailed += (n, c) => { authenticated = false; signal.Set(); };
                node.OnClientConnectionAuthenticationSucceeded += (n, c) => { authenticated = true; signal.Set(); };
                node.StartListening();

                var authenticator = new GossipDefaultAuthenticator(Encoding.UTF8.GetBytes("IncorrectClusterKey"));
                using (GossipConnection.ConnectAsync(nodeConfig.NodeId, nodeConfig.BindToEndPoint, authenticator, c => signal.Set()))
                {
                    signal.Wait();
                    Assert.That(authenticated == false);
                }
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
