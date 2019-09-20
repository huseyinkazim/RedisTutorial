using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisLikeTtz.Core
{
    public class RedisLogger
    {
        private IDatabase cache { get { return RedisConnectorHelper.Connection.GetDatabase(); } }
        private IConnectionMultiplexer muxer { get { return cache.Multiplexer; } }
        private StackExchangeRedisCacheClient _cacheClient { set; get; }
        private StackExchangeRedisCacheClient cacheClient { get { lock (locked) { if (_cacheClient == null && muxer != null && muxer.IsConnected && muxer.GetDatabase() != null) { _cacheClient = new StackExchangeRedisCacheClient(muxer, new NewtonsoftSerializer()); } return _cacheClient; } } }
        private object locked = new object();
        public bool InsertItem<T>(string key, T obj) where T : class
        {

            bool result = false;

            if (cacheClient != null)
            {
                List<RedisValue> lst = new List<RedisValue>();

                lst.Add(JsonConvert.SerializeObject(obj));

                result = cacheClient.Database.ListLeftPush(key, lst.ToArray()) > 0;
            }

            return result;
        }
        public int GetListSize(string key)
        {
            return (int)cache.ListLength(key);
        }
        public ConcurrentBag<T> GetListItemRange<T>(string key, int start, int chunksize) where T : class
        {
            ConcurrentBag<T> obj = default(ConcurrentBag<T>);
            try
            {
                if (cacheClient != null)
                {
                    var redisValues = cacheClient.Database.ListRange(key, start, (start + chunksize - 1));

                    if (redisValues.Length > 0)
                    {
                        obj = new ConcurrentBag<T>(Array.ConvertAll(redisValues, value => JsonConvert.DeserializeObject<T>(value)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return obj;
        }
        public bool RemoveList(string key)
        {
            
            cache.KeyDelete(key);

            if (cache.KeyExists(key))
                return false;
            else
                return true;
        }

        public void SnapshottingSave()
        {
            //cacheClient.Save
        }

    }
}
