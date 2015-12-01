using System;
using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace GossipSharp
{
    [ProtoContract]
    public abstract class GossipMessage
    {
        [ProtoIgnore]
        public DateTime Expires { get; set; }

        private DateTime _created = GossipTimestampProvider.CurrentTimestamp;
        [ProtoIgnore]
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        [ProtoIgnore]
        public int MessageType { get; set; }

        protected GossipMessage(int messageType)
        {
            MessageType = messageType;
        }

        public static T Deserialize<T>(byte[] rawData) where T : GossipMessage
        {
            using (var mem = new MemoryStream(rawData))
                return Serializer.Deserialize<T>(mem);
        }

        public virtual byte[] Serialize()
        {
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, this);
                var buf = mem.GetBuffer();
                var result = new byte[mem.Length];
                Array.Copy(buf, result, result.Length);
                return result;
            }
        }

        public async Task WriteToStreamAsync(BinaryNetworkStream stream)
        {
            var intBuf = BitConverter.GetBytes(MessageType);
            await stream.WriteAsync(intBuf, 0, intBuf.Length);

            var buf = Serialize();
            intBuf = BitConverter.GetBytes(buf.Length);
            await stream.WriteAsync(intBuf, 0, intBuf.Length);
            await stream.WriteAsync(buf, 0, buf.Length);
        }

        private static readonly byte[] _blankArray = new byte[0];
        public static async Task<GossipMessage> ReadFromStreamAsync(BinaryNetworkStream stream)
        {
            var intBuf = new byte[4];

            var read = await stream.ReadAsync(intBuf, 0, intBuf.Length);
            if (read != 4) return null;
            int messageType = BitConverter.ToInt32(intBuf, 0);

            read = await stream.ReadAsync(intBuf, 0, intBuf.Length);
            if (read != 4) return null;
            int size = BitConverter.ToInt32(intBuf, 0);
            if (size < 0) return null;
            if (size == 0) return GossipMessageFactory.CreateMessage(messageType, _blankArray);

            var result = new byte[size];
            int offset = 0;
            while (true)
            {
                read = await stream.ReadAsync(result, offset, size - offset);
                if (read == 0) return null;
                offset += read;
                if (offset >= size)
                    return GossipMessageFactory.CreateMessage(messageType, result);
            }
        }
    }
}
