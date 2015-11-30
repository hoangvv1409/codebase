using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Shard;
using Infrastructure.Shard.HashingFunction;
using Infrastructure.Shard.Repository;

namespace Infrastructure.Shard.SqlMasterSlave
{
    public class ShardService
    {
        private INodeRepository nodeRepository;
        private IHashing hashingFunction;
        public ClusterManager ClusterManager { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShardService"/> class.
        /// </summary>
        /// <param name="nodeRepository">Concrete Node repository</param>
        /// <param name="hashingFunction">Concrete Hashsing function</param>
        /// <param name="vNodes">Number of virtual nodes - default = 10</param>
        public ShardService(INodeRepository nodeRepository, IHashing hashingFunction, int vNodes = 10)
        {
            this.nodeRepository = nodeRepository;
            var nodes = nodeRepository.GetAllNode();

            if (nodes == null || nodes.Count == 0) throw new Exception("No available node, check your connection string or database");

            ClusterManager = new ClusterManager(nodes, hashingFunction, vNodes);
        }

        /// <summary>
        /// Adds new node to cluster
        /// </summary>
        /// <param name="connectionString">Nodes's connection string</param>
        public void AddNode(string connectionString)
        {
            var node = new Node(Guid.NewGuid(), connectionString);

            try
            {
                nodeRepository.AddToken(node);
                ClusterManager.AddNode(node);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Remove exist node
        /// </summary>
        /// <param name="nodeId">Node Id</param>
        public void RemoveNode(Guid nodeId)
        {
            var node = ClusterManager.GetNode(nodeId);

            try
            {
                nodeRepository.RemoveNode(node);
                ClusterManager.RemoveNode(node);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets node from cluster by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Node</returns>
        public Node GetNode(string key)
        {
            return ClusterManager.GetNode(key);
        }
    }
}