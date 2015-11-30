using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shard
{
    public class Token
    {
        public int Id { get; private set; }
        public int TokenKey { get; private set; }
        public Guid NodeId { get; private set; }

        //This constructor is used by Entity Framework
        private Token() { }

        public Token(int tokenKey, Guid nodeId)
        {
            if (nodeId == null) throw new ArgumentNullException("nodeId", "Node Id cannot be null");

            this.NodeId = nodeId;
            this.TokenKey = tokenKey;
        }
    }
}
