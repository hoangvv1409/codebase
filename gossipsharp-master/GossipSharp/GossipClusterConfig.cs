using System.Net;

namespace GossipSharp
{
    public class GossipClusterConfig : IGossipClusterConfig
    {
        public byte[] ClusterKey { get; set; }
        public IPEndPoint[] ClusterSeeds { get; set; }
    }
}
