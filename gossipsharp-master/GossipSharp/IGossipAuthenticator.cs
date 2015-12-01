namespace GossipSharp
{
    public interface IGossipAuthenticator
    {
        byte[] ClusterKey { get; }
        byte[] GenerateHash(byte[] challenge);
    }
}
