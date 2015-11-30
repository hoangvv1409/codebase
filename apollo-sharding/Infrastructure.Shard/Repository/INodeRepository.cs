using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shard.Repository
{
    public interface INodeRepository
    {
        List<Node> GetAllNode();
        Node GetNode(Guid nodeId);
        void AddNode(Node node);
        void AddToken(Node node);
        void RemoveNode(Node node);
    }
}