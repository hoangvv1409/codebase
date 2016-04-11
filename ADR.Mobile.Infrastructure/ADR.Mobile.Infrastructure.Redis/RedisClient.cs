using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Redis
{
    public class RedisClient : IDisposable
    {
        protected RedisConnectionSetting connectionSettings;
        protected RedisAddressSetting addressSettings;
        protected SocketManager _socketManager;
        protected ConnectionMultiplexer _connection;
        private readonly object SyncConnectionLock = new object();

        public string ServerIp { get; private set; }
        public int DbId { get; private set; }

        protected RedisClient(RedisConnectionSetting connectionSettings, RedisAddressSetting serverAddressSettings)
        {
            this.connectionSettings = connectionSettings;
            this.addressSettings = serverAddressSettings;
            this.ServerIp = this.addressSettings.ServerIP;
            this.DbId = this.addressSettings.DbId;

            _socketManager = new SocketManager(GetType().Name);
        }

        protected ConnectionMultiplexer GetRedisConnection()
        {
            lock (SyncConnectionLock)
            {
                if (_connection == null)
                    _connection = ConfigConnection();
                if (!_connection.IsConnected)
                    _connection = ConfigConnection();
                if (_connection.IsConnected)
                    return _connection;
                return _connection;
            }
        }

        private ConnectionMultiplexer ConfigConnection()
        {
            var config = ConfigurationOptions.Parse(this.addressSettings.ServerIP);
            config.KeepAlive = this.connectionSettings.KeepAlive;
            config.SyncTimeout = this.connectionSettings.SyncTimeout;
            config.AbortOnConnectFail = this.connectionSettings.AbortOnConnectionFail;
            config.AllowAdmin = this.connectionSettings.AllowAdmin;
            config.ConnectTimeout = this.connectionSettings.ConnectionTimeout;
            config.SocketManager = _socketManager;
            config.ConnectRetry = this.connectionSettings.ConnectRetry;

            var connection = ConnectionMultiplexer.ConnectAsync(config);
            var muxer = connection.Result;

            return muxer;
        }

        protected IDatabase GetDatabase()
        {
            return this.GetRedisConnection().GetDatabase(this.DbId);
        }

        protected IServer GetServer()
        {
            return this.GetRedisConnection().GetServer(this.ServerIp);
        }

        public void Dispose()
        {
            this._connection.Close();
            this._socketManager.Dispose();
        }
    }
}
