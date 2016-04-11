using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ADR.Mobile.Infrastructure.Redis
{
    public class RedisWriteClient : RedisClient
    {
        public RedisWriteClient(RedisCacheSetting settings)
            : base(settings.ConnectionSettings, settings.RedisMaster)
        { }

        /// <summary>
        /// Store data to the Cached with a key and value (format byte[]) in durationInMinute(minutes)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool Set(string key, RedisValue value, TimeSpan? duration = null)
        {
            RedisKey redisKey = key;
            this.GetDatabase().StringSet(redisKey, value, duration);

            //var connection = ConfigConnection().GetDatabase(1);
            //connection.StringSet(redisKey, value, timeSpanDuration);
            return true;
        }

        /// <summary>
        /// Xóa 1 key khỏi redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return this.GetDatabase().KeyDelete(key);

            //var connection = ConfigConnection().GetDatabase(1);
            //return connection.KeyDelete(key);
        }
    }
}
