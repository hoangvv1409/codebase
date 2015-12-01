using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipSharp
{
    class Program
    {
        static void Main()
        {
            Debug.Listeners.Add(new ConsoleTraceListener());

            var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
            var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Any, 30000), "tag1");
            var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30001), "tag2");
            var nodeConfig3 = new GossipNodeConfig(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30002), "tag3");

            var buffer = new byte[1024 * 1024];
            var bulkMessage = new RawGossipMessage(0, buffer);

            using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
            using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
            using (var node3 = new GossipNode(nodeConfig3, clusterConfig))
            {
                node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));
                //node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig3, node3.Authenticator));

                //node2.Cluster.Join(new GossipClusterMember(nodeConfig2.NodeId, nodeConfig1, node1.Authenticator));
                //node3.Cluster.Join(new GossipClusterMember(nodeConfig3.NodeId, nodeConfig1, node1.Authenticator));

                node1.OnMessageReceived += ProcessSimpleTextMessage;
                node2.OnMessageReceived += ProcessSimpleTextMessage;
                node3.OnMessageReceived += ProcessSimpleTextMessage;

                node1.StartListening();
                node2.StartListening();
                node3.StartListening();

                node1.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes("Node 1 Info"))).Wait();

                node2.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes("Node 2 Info"))).Wait();

                node3.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes("Node 3 Info"))).Wait();

                const int iterations = 1000;
                var sw = Stopwatch.StartNew();
                var tasks = new List<Task>();
                for (var i = 0; i < iterations; i++)
                {
                    tasks.Add(node1.Cluster.BroadcastMessageAsync(bulkMessage));
                }
                Task.WaitAll(tasks.ToArray());
               Console.WriteLine("{0} iterations of {1} KB transferred in {2}ms ({3:F1} ops/s, {4:F1} MB/s)", 
                    iterations, 
                    buffer.Length / 1024,
                    sw.ElapsedMilliseconds, 
                    iterations / (sw.ElapsedMilliseconds / 1000m), 
                    iterations * buffer.Length / (sw.ElapsedMilliseconds / 1000m) / 1024m / 1024m);
            }

            Console.ReadLine();
        }

        private static void ProcessSimpleTextMessage(GossipNode node, GossipConnection connection, GossipMessage message)
        {
            var rawMessage = message as RawGossipMessage;
            if (rawMessage == null) return;
            if (rawMessage.MessageType == 1)
            {
                Debug.WriteLine(String.Format("Node Info: {0}", node.NodeConfig.Tags.First()));
                Debug.WriteLine(String.Format("Node received: {0}", node.NodeConfig.NodeId.ToHexStringLower()));
                Debug.WriteLine("Message Size: {0} bytes", rawMessage.Size);
                Debug.WriteLine(String.Format("Message String: {0}", Encoding.UTF8.GetString(rawMessage.Buffer)));
            }
        }
    }
}
