using System;
using System.Security.Cryptography;

namespace GossipSharp
{
    public class GossipDefaultAuthenticator : IGossipAuthenticator
    {
        public byte[] ClusterKey { get; private set; }

        public GossipDefaultAuthenticator(byte[] clusterKey)
        {
            ClusterKey = clusterKey;
        }

        public byte[] GenerateHash(byte[] challenge)
        {
            if (challenge == null) throw new ArgumentNullException("challenge");
            if (challenge.Length != 8) throw new InvalidOperationException("Invalid challenge size");

            using (var sha1 = SHA1.Create())
            {
                sha1.TransformBlock(challenge, 0, challenge.Length, challenge, 0);
                sha1.TransformFinalBlock(ClusterKey, 0, ClusterKey.Length);
                return sha1.Hash;
            }
        }
    }
}
