using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ADR.Mobile.Infrastructure.Redis
{
    public class RedisReadClient : RedisClient
    {
        public RedisReadClient(RedisCacheSetting settings)
            : base(settings.ConnectionSettings, settings.RedisSlave)
        { }

        /// <summary>
        /// Gets a value from redis by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue Get(string key)
        {
            return this.GetDatabase().StringGet(key);
            //var connection = ConfigConnection().GetDatabase(1);
            //return connection.StringGet(key);
        }

        /// <summary>
        /// Lấy danh sách key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="offSet"></param>
        public IEnumerable<RedisKey> GetKeys(string key, int pageSize, int offSet)
        {
            return this.GetServer().Keys(this.DbId, key, pageSize, 0, offSet);
        }
    }
}
