using System;
using System.Threading.Tasks;

namespace GossipSharp
{
    public sealed class GossipConnectionSession : IDisposable
    {
        public GossipConnection Connection { get; private set; }
        private Action<GossipConnection> _disposeAction;
        public Action<GossipConnection> DisposeAction { get { return _disposeAction; } set { _disposeAction = value; } }

        private GossipConnectionSession(GossipConnection connection)
        {
            Connection = connection;
        }

        public static GossipConnectionSession FromOpenConnection(GossipConnection connection, Action<GossipConnection> disposeAction = null)
        {
            var session = new GossipConnectionSession(connection);
            session._disposeAction = disposeAction;
            return session;
        }

        public async Task<bool> SendMessageAsync(GossipMessage message)
        {
            if (Connection == null) return false;
            await message.WriteToStreamAsync(Connection.Stream);
            return true;
        }

        public void Dispose()
        {
            if (_disposeAction != null)
                _disposeAction(Connection);
        }
    }
}
