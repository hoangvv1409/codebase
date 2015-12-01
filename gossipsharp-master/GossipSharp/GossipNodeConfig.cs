using System;
using System.Diagnostics;
using System.Net;

namespace GossipSharp
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class GossipNodeConfig : IGossipNodeConfig, ICloneable
    {
        public ulong NodeId { get; set; }
        public IPEndPoint BindToEndPoint { get; set; }

        private string[] _tags;
        public string[] Tags
        {
            get { return _tags; }
            set { _tags = value ?? new string[0]; }
        }

        public GossipNodeConfig()
        {
        }

        public GossipNodeConfig(IPEndPoint bindToEndPoint, params string[] tags)
        {
            BindToEndPoint = bindToEndPoint;
            NodeId = BindToEndPoint.ToString().GetHashCodeLong();
            Tags = tags ?? new string[0];
        }

        // ReSharper disable UnusedMember.Local
        private string DebuggerDisplay
        {
            get { return String.Format("NodeId: {0}, EndPoint: {1}, Tags: {2}", NodeId.ToHexStringLower(), BindToEndPoint, String.Join(",", Tags)); }
        }
        // ReSharper restore UnusedMember.Local

        public GossipNodeConfig Clone()
        {
            return (GossipNodeConfig)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }
}
