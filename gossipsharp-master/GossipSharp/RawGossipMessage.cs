using System;
using ProtoBuf;

namespace GossipSharp
{
    [ProtoContract]
    public class RawGossipMessage : GossipMessage
    {
        private static readonly byte[] _blankArray = new byte[0];

        public int Size { get { return _buffer.Length; } }

        private byte[] _buffer;
        [ProtoMember(1)]
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value ?? _blankArray; }
        }

        public RawGossipMessage() : base(0)
        {
            Buffer = new byte[0];
        }

        public RawGossipMessage(int messageType) : base(messageType)
        {
            Buffer = _blankArray;
        }

        public RawGossipMessage(byte[] buffer) : base(0)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            Buffer = buffer;
        }

        public RawGossipMessage(int messageType, byte[] buffer)
            : base(messageType)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            Buffer = buffer;
        }

        public override byte[] Serialize()
        {
            return _buffer;
        }
    }
}
