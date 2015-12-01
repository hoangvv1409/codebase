using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GossipSharp
{
    public class GossipCluster
    {
        public IGossipNodeConfig NodeConfig { get; private set; }
        public IGossipClusterConfig ClusterConfig { get; private set; }

        private readonly ConcurrentDictionary<ulong, GossipClusterMember> _members = new ConcurrentDictionary<ulong, GossipClusterMember>();
        public event Action<GossipCluster, GossipClusterMember> OnNewMember = (cluster, member) => { };

        public GossipCluster(IGossipNodeConfig nodeConfig, IGossipClusterConfig clusterConfig)
        {
            if (nodeConfig == null) throw new ArgumentNullException("nodeConfig");
            if (clusterConfig == null) throw new ArgumentNullException("clusterConfig");

            NodeConfig = nodeConfig;
            ClusterConfig = clusterConfig;
        }

        public void Join(GossipClusterMember member)
        {
            if (member == null) throw new ArgumentNullException("member");
            bool added = false;
            _members.GetOrAdd(member.RemoteNodeId, id =>
                                                       {
                                                           added = true;
                                                           return member;
                                                       });
            if (added) OnNewMember(this, member);
        }

        public GossipClusterMember Join(GossipConnection connection, IPEndPoint endPoint, IGossipAuthenticator authenticator, params string[] tags)
        {
            var clusterMember = _members.GetOrAdd(connection.RemoteNodeId, id => new GossipClusterMember(NodeConfig.NodeId, connection.RemoteNodeId, endPoint, authenticator, tags));
            clusterMember.AttachOpenConnection(connection);
            return clusterMember;
        }

        public int BroadcastMessage(GossipMessage message, Func<GossipClusterMember, bool> selector = null)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (selector == null) selector = m => true;

            var waitTasks = new List<Task>();
            var resultTasks = new List<Task<bool>>();
            foreach (var member in _members.Values.Where(selector))
            {
                var task = member.SendMessageAsync(message);
                waitTasks.Add(task);
                resultTasks.Add(task);
            }
            Task.WaitAll(waitTasks.ToArray());
            return resultTasks.Count(x => x.Result);
        }

        public async Task BroadcastMessageAsync(GossipMessage message, Func<GossipClusterMember, bool> selector = null)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (selector == null) selector = m => true;

            var tasks = new List<Task>();
            foreach (var member in _members.Values.Where(selector))
            {
                tasks.Add(member.SendMessageAsync(message));
            }
            await Task.Run(() => 
                Task.WaitAll(tasks.ToArray()));
        }

        public GossipClusterMember this[ulong nodeId]
        {
            get
            {
                GossipClusterMember member;
                _members.TryGetValue(nodeId, out member);
                return member;
            }
        }
    }
}
