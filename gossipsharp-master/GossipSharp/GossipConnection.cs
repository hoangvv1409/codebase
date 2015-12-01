using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GossipSharp
{
    public class GossipConnection : IDisposable
    {
        public ulong LocalNodeId { get; set; }
        public ulong RemoteNodeId { get; set; }

        public TcpClient Client { get; protected set; }
        public BinaryNetworkStream Stream { get; protected set; }

        public EndPoint RemoteEndPoint { get; protected set; }

        public bool IsAuthenticated { get; protected set; }

        public DateTime TimestampConnected { get; protected set; }
        public DateTime TimestampLastActivity { get; protected set; }

        public event Action<GossipConnection> OnDisconnected = c => Debug.WriteLine("Remote connection " + c.RemoteNodeId.ToHexStringLower() + " disconnected " + c.RemoteEndPoint);
        public event Action<GossipConnection, GossipMessage> OnMessageReceived = (c, m) => { };
        public event Action<GossipConnection, Exception> OnException = (c, ex) => { };

        public GossipConnection(TcpClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            Client = client;
            RemoteEndPoint = client.Client.RemoteEndPoint;
            TimestampConnected = GossipTimestampProvider.CurrentTimestamp;
            TimestampLastActivity = TimestampConnected;

            Stream = new BinaryNetworkStream(client.GetStream());
            Stream.OnDisconnected += s => OnDisconnected(this);
        }

        private bool _startedReadingMessageData;
        protected async void StartReadingMessageData()
        {
            if (_startedReadingMessageData)
                throw new InvalidOperationException("Already started reading message data");

            _startedReadingMessageData = true;
            try
            {
                while (Client.Connected)
                {
                    var message = await GossipMessage.ReadFromStreamAsync(Stream);
                    if (message == null) break;
                    RecordActivity();

                    OnMessageReceived(this, message);
                }
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                OnException(this, ex);
            }
            finally
            {
                OnDisconnected(this);
            }
        }

        public async Task<bool?> RequestAuthentication(IGossipAuthenticator authenticator)
        {
            var randomNumber = RandomNumbers.Complex(5);
            var challenge = BitConverter.GetBytes(randomNumber);

            await Stream.WriteAsync(LocalNodeId);
            await Stream.WriteAsync(challenge, 0, challenge.Length);
            RecordActivity();

            var expected = authenticator.GenerateHash(challenge);

            RemoteNodeId = await Stream.ReadUInt64Async();

            var response = new byte[expected.Length];
            var read = await Stream.ReadAsync(response, 0, response.Length);
            if (read == 0) return null;
            RecordActivity();
            if (read != response.Length) return false;
            if (!expected.SequenceEqual(response)) return false;

            StartReadingMessageData();
            IsAuthenticated = true;
            return true;
        }

        public static async Task<GossipConnection> ConnectAsync(ulong localNodeId, IPEndPoint remoteEndPoint, IGossipAuthenticator authenticator, Action<GossipConnection> onDisconnected = null)
        {
            var client = new TcpClient();
            await client.ConnectAsync(remoteEndPoint.Address, remoteEndPoint.Port);
            var connection = new GossipConnection(client);
            connection.LocalNodeId = localNodeId;
            if (onDisconnected != null) connection.OnDisconnected += onDisconnected;
            await connection.RespondToAuthenticationRequestAsync(authenticator);
            return connection;
        }

        private async Task RespondToAuthenticationRequestAsync(IGossipAuthenticator authenticator)
        {
            RemoteNodeId = await Stream.ReadUInt64Async();
            RecordActivity();

            var challenge = new byte[8];
            var read = await Stream.ReadAsync(challenge, 0, challenge.Length);
            if (read != 8) return;

            await Stream.WriteAsync(LocalNodeId);

            byte[] sendBuffer = authenticator.GenerateHash(challenge);
            await Stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
            RecordActivity();

            IsAuthenticated = true;
            StartReadingMessageData();
        }

        private int _closed;
        public void Close()
        {
            var closed = Interlocked.Exchange(ref _closed, 1);
            if (closed != 0) return;

            Stream.Close();
            Client.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public void RecordActivity()
        {
            TimestampLastActivity = GossipTimestampProvider.CurrentTimestamp;
        }
    }
}
