using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GossipSharp
{
    public class BinaryNetworkStream : Stream
    {
        private readonly NetworkStream _baseStream;
        private readonly byte[] _readBuffer = new byte[4096];

        public event Action<BinaryNetworkStream> OnDisconnected = s => { };

        public BinaryNetworkStream(NetworkStream baseStream)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            _baseStream = baseStream;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public new async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await _baseStream.ReadAsync(buffer, offset, count);
        }

        public ulong ReadUInt64()
        {
            int offset = 0;
            const int expected = sizeof (ulong);
            while (true)
            {
                var read = _baseStream.Read(_readBuffer, offset, expected - offset);
                if (read == 0)
                {
                    OnDisconnected(this);
                    return 0;
                }
                offset += read;
                if (offset >= expected)
                    return BitConverter.ToUInt64(_readBuffer, 0);
            }
        }

        public async Task<ulong> ReadUInt64Async()
        {
            int offset = 0;
            const int expected = sizeof(ulong);
            while (true)
            {
                var read = await _baseStream.ReadAsync(_readBuffer, offset, expected - offset);
                if (read == 0)
                {
                    OnDisconnected(this);
                    return 0;
                }
                offset += read;
                if (offset >= expected)
                    return BitConverter.ToUInt64(_readBuffer, 0);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        public new async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await _baseStream.WriteAsync(buffer, offset, count);
        }

        public void Write(ulong value)
        {
            var buf = BitConverter.GetBytes(value);
            _baseStream.Write(buf, 0, buf.Length);
        }

        public async Task WriteAsync(ulong value)
        {
            var buf = BitConverter.GetBytes(value);
            await _baseStream.WriteAsync(buf, 0, buf.Length);
        }

        public bool DataAvailable
        {
            get { return _baseStream.DataAvailable; }
        }

        public override bool CanRead
        {
            get { return _baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _baseStream.CanWrite; }
        }

        public override int ReadTimeout
        {
            get { return _baseStream.ReadTimeout; }
            set { _baseStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return _baseStream.WriteTimeout; }
            set { _baseStream.WriteTimeout = value; }
        }

        public override long Length
        {
            get { return _baseStream.Length; }
        }

        public override long Position { get { return _baseStream.Position; } set { _baseStream.Position = value; } }
    }
}
