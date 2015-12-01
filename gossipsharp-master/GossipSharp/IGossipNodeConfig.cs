using System.Net;

namespace GossipSharp
{
    public interface IGossipNodeConfig
    {
        ulong NodeId { get; }
        IPEndPoint BindToEndPoint { get; }
        string[] Tags { get; }
    }
}
