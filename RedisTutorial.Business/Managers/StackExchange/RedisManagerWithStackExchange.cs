using Newtonsoft.Json;
using StackExchange.Redis;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Managers.StackExchange
{
    public class RedisManagerWithStackExchange
    {
        private IDatabase database => _connection.GetDatabase();
        private ConnectionMultiplexer _connection => RedisConnector.TakeInstance();
        //private IConnectionMultiplexer muxer => database.Multiplexer;
        //private StackExchangeRedisCacheClient _cacheClient { set; get; }
        //public StackExchangeRedisCacheClient cacheClient { get { lock (locked) { if (_cacheClient == null && muxer != null && muxer.IsConnected && muxer.GetDatabase() != null) { _cacheClient = new StackExchangeRedisCacheClient(muxer, new NewtonsoftSerializer()); } return _cacheClient; } } }
        private object locked = new object();
        public bool InsertItem<T>(string key, T obj) where T : class
        {

            bool result = false;
            if (database.Multiplexer.IsConnected)
            {
                List<RedisValue> lst = new List<RedisValue>();

                lst.Add(JsonConvert.SerializeObject(obj));

                result = database.ListLeftPush(key, lst.ToArray()) > 0;
            }

            return result;
        }

        public int GetListSize(string key)
        {
            return (int)database.ListLength(key);
        }
        public ConcurrentBag<T> GetListItemRange<T>(string key, int start = 0, int chunksize = 0) where T : class
        {
            chunksize = chunksize != 0 ? chunksize : GetListSize(key);
            ConcurrentBag<T> obj = default(ConcurrentBag<T>);
            try
            {
                if (database.Multiplexer.IsConnected)
                {
                    var redisValues = database.ListRange(key, start, (start + chunksize - 1));

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

            database.KeyDelete(key);

            if (database.KeyExists(key))
                return false;
            else
                return true;
        }
    }
}
