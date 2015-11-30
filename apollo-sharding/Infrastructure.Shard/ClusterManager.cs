using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Shard.HashingFunction;

namespace Infrastructure.Shard
{
    public class ClusterManager
    {
        private IHashing hashingFunction;
        private SortedDictionary<int, Node> ring;
        private int[] ayKeys = null;    //cache the ordered keys for better performance

        public Dictionary<Guid, Node> Nodes { get; private set; }
        public int vNodes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterManager"/> class.
        /// </summary>
        /// <param name="nodeRepository">Concrete Node repository</param>
        /// <param name="hashingFunction">Concrete Hashsing function</param>
        /// <param name="vNodes">Number of virtual nodes - default = 10</param>
        public ClusterManager(IEnumerable<Node> nodes, IHashing hashingFunction, int vNodes = 10)
        {
            if (vNodes <= 0) throw new Exception("vNodes must greater than 0");
            if (nodes == null || !nodes.Any()) throw new Exception("No available node");

            this.hashingFunction = hashingFunction;
            this.vNodes = vNodes;
            this.ring = new SortedDictionary<int, Node>();
            this.Nodes = new Dictionary<Guid, Node>();

            foreach (var node in nodes)
            {
                AddNode(node);
            }
            ayKeys = ring.Keys.ToArray();
        }

        /// <summary>
        /// Add new node to cluster
        /// </summary>
        /// <param name="node">New node</param>
        public void AddNode(Node node)
        {
            if (Nodes.ContainsKey(node.NodeID)) throw new Exception("This node is exist");
            AddNode(node, true);
        }

        /// <summary>
        /// Remove exist node
        /// </summary>
        /// <param name="node">Node</param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node.NodeID);

            for (int i = 0; i < vNodes; i++)
            {
                int token = Hash(node.GetHashCode().ToString() + i);
                if (!ring.Remove(token))
                {
                    throw new Exception("can not remove a node that not added");
                }
            }
            ayKeys = ring.Keys.ToArray();
        }

        /// <summary>
        /// Gets node from cluster by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Node</returns>
        public Node GetNode(string key)
        {
            int token = Hash(key);

            int begin = 0;
            int end = ayKeys.Length - 1;

            if (ayKeys[end] < token || ayKeys[0] > token)
            {
                return ring[ayKeys[0]];
            }

            int mid = begin;
            while (end - begin > 1)
            {
                mid = (end + begin) / 2;
                if (ayKeys[mid] >= token)
                {
                    end = mid;
                }
                else
                {
                    begin = mid;
                }
            }

            if (ayKeys[begin] > token || ayKeys[end] < token)
            {
                throw new Exception("should not happen");
            }

            return ring[ayKeys[end]];
        }

        /// <summary>
        /// Gets node from cluster by node id
        /// </summary>
        /// <param name="nodeId">Node Id</param>
        /// <returns>Node</returns>
        public Node GetNode(Guid nodeId)
        {
            return Nodes[nodeId];
        }

        #region Outside Request Method
        /// <summary>
        /// Post request (Update, Insert) to shard cluster
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="postFunc">The post (update, insert) function</param>
        public void Post(string key, Action<string> postFunc)
        {
            var node = GetNode(key);
            if (node != null)
            {
                postFunc.Invoke(node.ConnectionString);
            }
        }

        /// <summary>
        /// Get request (Select) from shard cluster
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="getFunc">The get (Select) function</param>
        /// <returns>Specific data</returns>
        public T Get<T>(string key, Func<string, T> getFunc)
        {
            var node = GetNode(key);
            if (node != null)
            {
                return getFunc.Invoke(node.ConnectionString);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Delete request to shard cluster
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="deleteFunc">The delete function</param>
        public void Delete(string key, Action<string> deleteFunc)
        {
            var node = GetNode(key);
            if (node != null)
            {
                deleteFunc.Invoke(node.ConnectionString);
            }
        }
        #endregion

        #region Private Method
        private void AddNode(Node node, bool updateKeyArray = false)
        {
            List<Token> tokens = new List<Token>();
            for (int i = 0; i < vNodes; i++)
            {
                int token = Hash(node.GetHashCode().ToString() + i);
                tokens.Add(new Token(token, node.NodeID));
                ring[token] = node;
            }

            if (updateKeyArray)
            {
                ayKeys = ring.Keys.ToArray();
            }

            node.AssignToken(tokens);
            Nodes.Add(node.NodeID, node);
        }

        private int Hash(String key)
        {
            uint token = hashingFunction.Hash(Encoding.ASCII.GetBytes(key));
            return (int)token;
        }
        #endregion
    }
}
