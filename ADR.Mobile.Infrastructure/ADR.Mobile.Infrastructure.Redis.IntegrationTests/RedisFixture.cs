using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADR.Mobile.Infrastructure.Redis.IntegrationTests
{
    [TestClass]
    public class RedisFixture : GivenARedisConfig
    {
        private RedisWriteClient redisWriteClient;
        private RedisReadClient redisReadClient;
        private string existKey = "Test_Exist_Key";
        private string newKey = "Test_Non_Exist_Key";
        private string prepareValue;

        public RedisFixture()
        {
            this.redisWriteClient = new RedisWriteClient(this.RedisCacheSetting);
            this.redisReadClient = new RedisReadClient(this.RedisCacheSetting);

            #region Setup
            this.redisWriteClient.Remove(newKey);
            this.redisWriteClient.Remove(existKey);

            prepareValue = Guid.NewGuid().ToString();
            this.redisWriteClient.Set(existKey, prepareValue);

            this.redisWriteClient.Set(existKey + "_1", prepareValue);
            this.redisWriteClient.Set(existKey + "_2", prepareValue);
            this.redisWriteClient.Set(existKey + "_3", prepareValue);
            this.redisWriteClient.Set(existKey + "_4", prepareValue);
            this.redisWriteClient.Set(existKey + "_5", prepareValue);
            this.redisWriteClient.Set(existKey + "_6", prepareValue);
            this.redisWriteClient.Set(existKey + "_7", prepareValue);
            #endregion
        }

        [TestMethod]
        public void when_set_new_key_then_key_created_with_value_success()
        {
            string newValue = Guid.NewGuid().ToString();
            this.redisWriteClient.Set(newKey, newValue);

            var value = this.redisReadClient.Get(newKey);
            Assert.IsTrue(value == newValue);
        }

        [TestMethod]
        public void when_set_exist_key_then_replace_value_success()
        {
            var value = this.redisReadClient.Get(existKey);
            Assert.IsTrue(value == prepareValue);

            string newValue = Guid.NewGuid().ToString();
            this.redisWriteClient.Set(existKey, newValue);

            value = this.redisReadClient.Get(existKey);
            Assert.IsTrue(value == newValue);
        }

        [TestMethod]
        public void when_set_key_with_expired_time_then_key_auto_removed_in_time()
        {
            string newValue = Guid.NewGuid().ToString();
            this.redisWriteClient.Set(newKey, newValue, new TimeSpan(0, 0, 5));

            var value = this.redisReadClient.Get(newKey);
            Assert.IsTrue(value == newValue);

            Thread.Sleep(5000);

            value = this.redisReadClient.Get(newKey);
            Assert.IsTrue(value.IsNull);
            Assert.IsTrue(value.IsNullOrEmpty);
        }

        [TestMethod]
        public void when_remove_exist_key_then_success()
        {
            var value = this.redisReadClient.Get(existKey);
            Assert.IsTrue(value == prepareValue);

            this.redisWriteClient.Remove(existKey);

            var redisValue = this.redisReadClient.Get(existKey);
            Assert.IsTrue(redisValue.IsNull);
            Assert.IsTrue(redisValue.IsNullOrEmpty);
        }

        [TestMethod]
        public void when_remove_non_exist_key_then_success()
        {
            var redisValue = this.redisReadClient.Get(newKey);
            Assert.IsTrue(redisValue.IsNull);
            Assert.IsTrue(redisValue.IsNullOrEmpty);

            this.redisWriteClient.Remove(newKey);

            redisValue = this.redisReadClient.Get(newKey);
            Assert.IsTrue(redisValue.IsNull);
            Assert.IsTrue(redisValue.IsNullOrEmpty);
        }

        [TestMethod]
        public void when_get_list_key_then_success()
        {
            var keys = this.redisReadClient.GetKeys(existKey, 1, 10);
            Assert.IsTrue(keys.Count() == 1);
            Assert.IsTrue(keys.First() == existKey);


            keys = this.redisReadClient.GetKeys(existKey + "_*", 1, 4);
            foreach (var key in keys)
            {
                int count = keys.Count();
            }
            Assert.IsTrue(keys.Count() == 5);
            //Assert.IsTrue(keys.First() == existKey);
        }
    }
}
