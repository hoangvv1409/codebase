using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Shard;
using Infrastructure.Shard.Repository;

namespace Infrastructure.Shard.SqlMasterSlave
{
    public class NodeRepository : INodeRepository
    {
        private string masterConnectionString;

        public NodeRepository(string masterConnectionString)
        {
            this.masterConnectionString = masterConnectionString;
        }

        public List<Node> GetAllNode()
        {
            List<Node> nodes = new List<Node>();
            using (var ctx = new ShardClusterDBContext(masterConnectionString))
            {
                var result = (from n in ctx.Nodes
                              join t in ctx.Tokens on n.NodeID equals t.NodeId
                              select new
                              {
                                  NodeID = n.NodeID,
                                  ConnectionString = n.ConnectionString,
                                  Tokens = t.TokenKey
                              }).ToList();

                foreach (var r in result.GroupBy(g => new { g.NodeID, g.ConnectionString }, g => g.Tokens))
                {
                    List<Token> tokens = new List<Token>();
                    foreach (var value in r)
                    {
                        tokens.Add(new Token(value, r.Key.NodeID));
                    }

                    nodes.Add(new Node(r.Key.NodeID, r.Key.ConnectionString, tokens));
                }
            }

            return nodes;
        }

        public void AddNode(Node node)
        {
            using (var ctx = new ShardClusterDBContext(masterConnectionString))
            {
                ctx.Nodes.Add(node);
                ctx.SaveChanges();
            }
        }

        public void RemoveNode(Node node)
        {
            using (var ctx = new ShardClusterDBContext(masterConnectionString))
            {
                ctx.Nodes.Attach(node);
                ctx.Nodes.Remove(node);
                ctx.SaveChanges();
            }
        }

        public Node GetNode(Guid nodeId)
        {
            using (var ctx = new ShardClusterDBContext(masterConnectionString))
            {
                return (from n in ctx.Nodes
                        where n.NodeID == nodeId
                        select n).FirstOrDefault();
            }
        }

        public void AddToken(Node node)
        {
            using (var ctx = new ShardClusterDBContext(masterConnectionString))
            {
                ctx.Tokens.AddRange(node.Tokens);
                ctx.SaveChanges();
            }
        }
    }
}
