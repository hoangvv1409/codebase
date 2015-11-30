using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shard
{
    public class Node
    {
        public Guid NodeID { get; private set; }
        public virtual List<Token> Tokens { get; private set; }
        public string ConnectionString { get; private set; }

        //This constructor is used by Entity Framework
        private Node() { }

        public Node(Guid nodeId, string connectionString)
            : this()
        {
            if (nodeId == null) throw new ArgumentNullException("nodeId", "Node Id cannot be null");
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString", "Node Connection string cannot be null or empty");

            this.NodeID = nodeId;
            this.ConnectionString = connectionString;
        }

        public Node(Guid nodeId, string connectionString, List<Token> tokens)
            : this(nodeId, connectionString)
        {
            this.Tokens = tokens;
        }

        public void AssignToken(List<Token> tokens)
        {
            this.Tokens = tokens;
        }

        public override int GetHashCode()
        {
            return ("node_" + NodeID).GetHashCode();
        }
    }
}
