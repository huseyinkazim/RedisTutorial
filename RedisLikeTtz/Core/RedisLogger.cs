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
        public bool InsertItem<T>(string key, T obj) where T : class
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            IConnectionMultiplexer muxer = cache.Multiplexer;



            bool result = false;

            if (muxer != null && muxer.IsConnected && muxer.GetDatabase() != null)
            {
                var cacheClient = new StackExchangeRedisCacheClient(muxer, new NewtonsoftSerializer());


                List<RedisValue> lst = new List<RedisValue>();

                lst.Add(JsonConvert.SerializeObject(obj));

                result = cacheClient.Database.ListLeftPush(key, lst.ToArray()) > 0;


                //if (LogRedisRelatedActivities)
                //{
                //    Logger.InfoFormat("InsertItem => Key: {0}, Result: {1}", key, result);
                //}
            }

            return result;
        }
        public int GetListSize(string key)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            return (int)cache.ListLength(key);
        }
        public ConcurrentBag<T> GetListItemRange<T>(string key, int start, int chunksize) where T : class
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            IConnectionMultiplexer muxer = cache.Multiplexer;


            ConcurrentBag<T> obj = default(ConcurrentBag<T>);
            try
            {
                if (muxer != null && muxer.IsConnected)
                {
                    var cacheClient = new StackExchangeRedisCacheClient(muxer, new NewtonsoftSerializer());

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
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            cache.KeyDelete(key);

            if (cache.KeyExists(key))
                return false;
            else
                return true;
        }
    }
}
