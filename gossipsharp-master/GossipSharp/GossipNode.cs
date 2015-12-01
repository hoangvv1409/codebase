using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GossipSharp
{
    public class GossipNode : IDisposable
    {
        public IGossipNodeConfig NodeConfig { get; private set; }
        public IGossipClusterConfig ClusterConfig { get { return Cluster.ClusterConfig; } }
        public IGossipAuthenticator Authenticator { get; private set; }

        public GossipCluster Cluster { get; private set; }

        private readonly TcpListener _tcpListener;
        private readonly ManualResetEventSlim _stop = new ManualResetEventSlim(true);
        private readonly ManualResetEventSlim _stopped = new ManualResetEventSlim(true);

        public event Action<GossipNode, Exception> OnAcceptException = (n, e) => Debug.WriteLine(e);
        public event Action<GossipNode, GossipConnection> OnClientConnectionAccepted = (n, c) => Debug.WriteLine("Cluster incoming connection accepted from " + c.RemoteEndPoint);
        public event Action<GossipNode, GossipConnection> OnClientConnectionAuthenticationFailed = (n, c) => Debug.WriteLine("Cluster incoming connection failed to authenticate " + c.RemoteEndPoint);
        public event Action<GossipNode, GossipConnection> OnClientConnectionAuthenticationSucceeded = (n, c) => Debug.WriteLine("Cluster incoming connection " + c.RemoteNodeId.ToHexStringLower() + " authenticated " + c.RemoteEndPoint);
        public event Action<GossipNode, GossipConnection, GossipMessage> OnMessageReceived = (n, c, m) => { };

        public GossipNode(IGossipNodeConfig nodeConfig, IGossipClusterConfig clusterConfig)
        {
            if (nodeConfig == null) throw new ArgumentNullException("nodeConfig");

            NodeConfig = nodeConfig;
            Cluster = new GossipCluster(nodeConfig, clusterConfig);
            Cluster.OnNewMember += (c, m) => m.OnConnectionSuccess += (mem, conn) => conn.OnMessageReceived += (gc, msg) => OnMessageReceived(this, gc, msg);
            Authenticator = new GossipDefaultAuthenticator(clusterConfig.ClusterKey);

            if (nodeConfig.BindToEndPoint != null)
                _tcpListener = new TcpListener(nodeConfig.BindToEndPoint);
        }

        private readonly object _startStopLock = new object();
        public void StartListening()
        {
            lock (_startStopLock)
            {
                if (!_stopped.IsSet)
                    throw new InvalidOperationException("Already started");

                if (_tcpListener == null)
                    throw new InvalidOperationException("BindToEndPoint cannot be null in NodeConfig");

                _stop.Reset();
                _tcpListener.Start();

                Debug.WriteLine("Gossip cluster node " + NodeConfig.NodeId.ToHexStringLower() + " listening at " + _tcpListener.LocalEndpoint + "...");
                AcceptIncomingClients();
            }
        }

        public void StopListening()
        {
            lock (_startStopLock)
            {
                if (_tcpListener == null) return;

                if (!_stop.IsSet)
                    _tcpListener.Stop();

                _stop.Set();
                _stopped.Wait();
                Debug.WriteLine("Gossip cluster node " + NodeConfig.NodeId.ToHexStringLower() + " stopped listening at " + _tcpListener.LocalEndpoint + "...");
            }
        }

        private async void AcceptIncomingClients()
        {
            try
            {
                while (!_stop.Wait(0))
                {
                    try
                    {
                        var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                        var gossipConnection = new GossipConnection(tcpClient) { LocalNodeId = NodeConfig.NodeId };
                        OnClientConnectionAccepted(this, gossipConnection);

                        var authenticationAwaiter = gossipConnection.RequestAuthentication(Authenticator).GetAwaiter();
                        authenticationAwaiter.OnCompleted(() => ProcessAuthenticationResponse(authenticationAwaiter, gossipConnection));
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnAcceptException(this, ex);
                    }
                }
            }
            finally
            {
                _stopped.Set();
            }
        }

        private void ProcessAuthenticationResponse(TaskAwaiter<bool?> authenticationAwaiter, GossipConnection gossipConnection)
        {
            bool? result;
            try
            {
                result = authenticationAwaiter.GetResult();
            }
            catch (IOException)
            {
                return;
            }
            if (!result.HasValue) return;
            if (!result.Value)
            {
                OnClientConnectionAuthenticationFailed(this, gossipConnection);
                return;
            }
            ClientConnectionAuthenticationSucceeded(gossipConnection);
            OnClientConnectionAuthenticationSucceeded(this, gossipConnection);
        }

        private void ClientConnectionAuthenticationSucceeded(GossipConnection gossipConnection)
        {
            var gossipMember = Cluster.Join(gossipConnection, new IPEndPoint(IPAddress.Any, 0), Authenticator);
            gossipConnection.OnMessageReceived += (gc, msg) => OnMessageReceived(this, gc, msg);
            gossipMember.OnConnectionSuccess += (m, c) =>
                c.OnMessageReceived += (gc, msg) => OnMessageReceived(this, gc, msg);
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
                StopListening();
            }
        }
    }
}
