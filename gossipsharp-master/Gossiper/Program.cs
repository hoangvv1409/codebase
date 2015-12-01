using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipSharp;

namespace Gossiper
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes("ClusterKey") };
                var nodeConfig1 = new GossipNodeConfig(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30001), "tag2");
                var nodeConfig2 = new GossipNodeConfig(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30002), "tag2");
                using (var node2 = new GossipNode(nodeConfig2, clusterConfig))
                using (var node1 = new GossipNode(nodeConfig1, clusterConfig))
                {
                    node1.Cluster.Join(new GossipClusterMember(nodeConfig1.NodeId, nodeConfig2, node2.Authenticator));

                    node1.OnMessageReceived += ProcessSimpleTextMessage;

                    node1.StartListening();

                    node1.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes("Node 1 Info"))).Wait();
                }

                Console.ReadLine();
                //var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                //string startPort = ConfigurationManager.AppSettings["StartPort"];

                //if (startPort != "")
                //{
                //    Console.WriteLine("Join Cluster");
                //    var startEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(startPort));
                //    Node node = new Node(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port), "ClusterKey", startEndPoint);
                //}
                //else
                //{
                //    Console.WriteLine("First Seed");
                //    Node node = new Node(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port), "ClusterKey");
                //}
            }
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
