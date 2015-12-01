using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GossipSharp;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Gossiper
{
    public class Node
    {
        public List<string> Tokens { get; private set; }
        public GossipNode Gossip { get; private set; }
        public List<RingInfo> RingInfos { get; private set; }

        private GossipNodeConfig gossipConfig;
        private GossipClusterConfig clusterConfig;

        public Node(IPEndPoint ipEndpoint, string clusterKey)
        {
            clusterConfig = new GossipClusterConfig { ClusterKey = Encoding.UTF8.GetBytes(clusterKey) };
            gossipConfig = new GossipNodeConfig(ipEndpoint);
            Gossip = new GossipNode(gossipConfig, clusterConfig);

            Gossip.OnMessageReceived += ProcessInfo;
        }

        public Node(IPEndPoint ipEndpoint, string clusterKey, List<RingInfo> ringInfos)
            : this(ipEndpoint, clusterKey)
        {
            RingInfos = ringInfos;

            Task.Factory.StartNew(() => StartListening());
            Task.Factory.StartNew(() => StartGossip());
        }

        public Node(IPEndPoint ipEndpoint, string clusterKey, IPEndPoint startEndPoint = null)
            : this(ipEndpoint, clusterKey)
        {
            RingInfos = new List<RingInfo>();

            if (startEndPoint != null)
            {
                RingInfos.Add(new RingInfo()
                {
                    IP = startEndPoint,
                    Status = true
                });
            }

            StartGossip();
        }

        private void StartGossip()
        {
            Timer timer = null;
            timer =
                new Timer(_ =>
                {
                    IPEndPoint ipEndPoint;
                    if (RingInfos.Count > 0)
                    {
                        if (RingInfos.Count > 1)
                        {
                            Random random = new Random();
                            var position = random.Next(0, RingInfos.Count);

                            ipEndPoint = RingInfos[position].IP;
                        }
                        else
                        {
                            ipEndPoint = RingInfos[0].IP;
                        }

                        JoinCluster(ipEndPoint);

                        Gossip.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes("Hello"))).Wait();
                    }
                });

            timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(500));
        }

        private void StartListening()
        {
            Gossip.StartListening();
        }

        private void JoinCluster(IPEndPoint ipEndPoint)
        {
            var contactNodeConfig = new GossipNodeConfig(ipEndPoint);
            var contactNode = new GossipNode(contactNodeConfig, clusterConfig);

            Gossip = new GossipNode(gossipConfig, clusterConfig);
            Gossip.Cluster.Join(new GossipClusterMember(gossipConfig.NodeId, contactNodeConfig, contactNode.Authenticator));
        }

        private void ProcessInfo(GossipNode node, GossipConnection connection, GossipMessage message)
        {
            var rawMessage = message as RawGossipMessage;
            string msg = Encoding.UTF8.GetString(rawMessage.Buffer);
            if (msg == "")
            {
                Console.WriteLine("First Handshake");

                JoinCluster(node.NodeConfig.BindToEndPoint);
                string senMsg = JsonConvert.SerializeObject(RingInfos);

                Gossip.Cluster.BroadcastMessageAsync(new RawGossipMessage(1, Encoding.UTF8.GetBytes(senMsg))).Wait();
            }
            else
            {
                Console.WriteLine("Second Handshake");

                var ringInfos = JsonConvert.DeserializeObject<List<RingInfo>>(msg);

                RingInfos = ringInfos;
            }
        }
    }
}