using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Manager
{
    public class RedisManager : IRedisManager
    {
        private IRedisNativeClient nativeClient;
        private IRedisClient redisClient;

        public RedisManager(IRedisNativeClient nativeClient, IRedisClient redisClient)
        {
            this.nativeClient = nativeClient;
            this.redisClient = redisClient;
        }
        public void SetString(string key, string value)
        {
            redisClient.SetValueIfNotExists(key, value);
        }
        public string GetString(string key)
        {

            
            return redisClient.GetValue(key);
        }
        public void SetList(string key, string[] list)
        {
            var names = redisClient.Lists[key];
            for (int i = 0; i < list.Length; i++)
                names.Add(list[i]);
        }
        public List<string> GetList(string key)
        {
            var names = redisClient.Lists[key].ToList();
            return names;

        }
    }
}
