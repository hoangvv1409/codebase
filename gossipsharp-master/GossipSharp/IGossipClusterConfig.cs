using System.Net;

namespace GossipSharp
{
    public interface IGossipClusterConfig
    {
        byte[] ClusterKey { get; }
        IPEndPoint[] ClusterSeeds { get; }
    }
}
