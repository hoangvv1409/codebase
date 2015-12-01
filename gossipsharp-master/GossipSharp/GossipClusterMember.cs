using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GossipSharp
{
    public class GossipClusterMember : IDisposable
    {
        internal static TimeSpan MinConnectionInactivity = TimeSpan.FromMinutes(1);

        private TimeSpan _connectionTimeout = TimeSpan.FromSeconds(3);
        public TimeSpan ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        public ulong LocalNodeId { get; private set; }
        public ulong RemoteNodeId { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public IGossipAuthenticator Authenticator { get; private set; }

        private TimeSpan _maxConnectionInactivity = TimeSpan.FromMinutes(5);
        public TimeSpan MaxConnectionInactivity
        {
            get { return _maxConnectionInactivity; }
            set
            {
                if (value < MinConnectionInactivity)
                    throw new ArgumentOutOfRangeException("value", "Max connection inactivity cannot be under " + MinConnectionInactivity);
                _maxConnectionInactivity = value;
                if (_timerRemoveInactiveConnections != null)
                {
                    var timeSpan = value;
                    if (timeSpan == TimeSpan.Zero) timeSpan = TimeSpan.FromMilliseconds(-1);
                    _timerRemoveInactiveConnections.Change(timeSpan, timeSpan);
                }
            }
        }

        private readonly BlockingCollection<GossipConnection> _availableConnections = new BlockingCollection<GossipConnection>();
        private readonly ConcurrentDictionary<GossipConnection, object> _allConnections = new ConcurrentDictionary<GossipConnection, object>();

        public int NumberOfAvailableConnections
        {
            get { return _availableConnections.Count; }
        }

        public int NumberOfOpenConnections
        {
            get { return _allConnections.Count; }
        }

        public event Action<GossipClusterMember, GossipMessage> OnMessageExpired = (mb, m) => { };
        public event Action<GossipClusterMember, GossipConnection> OnConnectionSuccess = (mb, c) => { };
        public event Action<GossipClusterMember, Exception> OnConnectionFailed = (mb, ex) => { };

        private int _consecutiveConnectionErrors;
        private readonly Timer _timerRemoveInactiveConnections;
        private DateTime _skipConnectingUntil = DateTime.MinValue;
        public DateTime SkipConnectingUntil { get { return _skipConnectingUntil; } set { _skipConnectingUntil = value; } }

        public HashSet<string> Tags { get; private set; }

        public GossipClusterMember(ulong localNodeId, IGossipNodeConfig config, IGossipAuthenticator authenticator)
            : this(localNodeId, config.NodeId, config.BindToEndPoint, authenticator, config.Tags)
        {
        }

        public GossipClusterMember(ulong localNodeId, ulong remoteNodeId, IPEndPoint remoteEndPoint, IGossipAuthenticator authenticator, params string[] tags)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");
            if (authenticator == null) throw new ArgumentNullException("authenticator");

            _maxConnectionInactivity = TimeSpan.FromMinutes(5);
            _timerRemoveInactiveConnections = new Timer(RemoveInactiveConnections, null, _maxConnectionInactivity, _maxConnectionInactivity);
            LocalNodeId = localNodeId;
            RemoteNodeId = remoteNodeId;
            RemoteEndPoint = remoteEndPoint;
            Authenticator = authenticator;
            Tags = new HashSet<string>((tags ?? new string[0]).Select(x => (x ?? "").ToLowerInvariant()));
        }

        public void RemoveInactiveConnections(object state)
        {
            var cutoffTimestamp = GossipTimestampProvider.CurrentTimestamp - _maxConnectionInactivity;

            var list = new List<GossipConnection>();
            GossipConnection connection;
            while (_availableConnections.TryTake(out connection, 0))
            {
                if (connection.TimestampLastActivity > cutoffTimestamp)
                    list.Add(connection);
                else
                {
                    object value;
                    if (_allConnections.TryRemove(connection, out value))
                        connection.Close();
                }
            }

            foreach (var readd in list)
                _availableConnections.Add(readd);
        }

        public GossipConnectionSession CreateSession()
        {
            GossipConnection connection;
            if (_allConnections.IsEmpty)
            {
                connection = OpenNewConnection();
            }
            else
            {
                if (!_availableConnections.TryTake(out connection, ConnectionTimeout))
                {
                    connection = OpenNewConnection();
                }
                else connection.RecordActivity();
            }
            return GossipConnectionSession.FromOpenConnection(connection, CloseSession);
        }

        public bool HasTag(string tag)
        {
            return tag != null && Tags.Contains(tag.ToLowerInvariant());
        }

        public async Task<bool> SendMessageAsync(GossipMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            if (message.Expires >= GossipTimestampProvider.CurrentTimestamp)
            {
                OnMessageExpired(this, message);
                return false;
            }

            using (var session = CreateSession())
            {
                if (message.Expires >= GossipTimestampProvider.CurrentTimestamp)
                {
                    OnMessageExpired(this, message);
                    return false;
                }

                try
                {
                    return await session.SendMessageAsync(message);
                }
                catch (IOException)
                {
                    session.DisposeAction = c => c.Close();
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    session.DisposeAction = c => c.Close();
                    return false;
                }
            }
        }

        private void CloseSession(GossipConnection connection)
        {
            if (connection == null) return;
            _availableConnections.Add(connection);
        }

        private GossipConnection OpenNewConnection()
        {
            if (_skipConnectingUntil != DateTime.MinValue &&
                GossipTimestampProvider.CurrentTimestamp < _skipConnectingUntil) return null;

            try
            {
                var task = GossipConnection.ConnectAsync(LocalNodeId, RemoteEndPoint, Authenticator, OnDisconnect);
                if (!task.Wait(ConnectionTimeout) || task.Result == null)
                {
                    Interlocked.Increment(ref _consecutiveConnectionErrors);
                    SkipNewConnectionsForAWhile();
                    OnConnectionFailed(this, new TimeoutException());
                    return null;
                }
                var connection = task.Result;
                _allConnections[connection] = null;
                OnConnectionSuccess(this, connection);
                return connection;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _consecutiveConnectionErrors);
                SkipNewConnectionsForAWhile();
                OnConnectionFailed(this, ex);
                return null;
            }
        }

        private void SkipNewConnectionsForAWhile()
        {
            var errors = _consecutiveConnectionErrors;
            if (errors > 100) errors = 100;
            var skipSeconds = 10 + (long)(Math.Pow(2, errors));
            if (skipSeconds > 600) skipSeconds = 600;
            _skipConnectingUntil = GossipTimestampProvider.CurrentTimestamp.AddSeconds(skipSeconds);
        }

        private void OnDisconnect(GossipConnection connection)
        {
            object value;
            _allConnections.TryRemove(connection, out value);
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
                _timerRemoveInactiveConnections.Dispose();
                DisconnectConnections();
            }
        }

        private void DisconnectConnections()
        {
            foreach (var connection in _allConnections.Keys)
                connection.Close();
        }

        public void AttachOpenConnection(GossipConnection connection)
        {
            _allConnections.GetOrAdd(connection, c => null);
            _availableConnections.Add(connection);

        }
    }
}
