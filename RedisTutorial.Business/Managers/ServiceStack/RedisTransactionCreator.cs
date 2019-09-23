using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Managers.ServiceStack
{
    public class RedisTransactionCreator
    {
        private static IRedisTransaction _redisTransaction;
        private static object locked=new object();

        public static IRedisTransaction CreateInstance(IRedisClient client)
        {
            lock (locked)
            {
                if (_redisTransaction == null)
                {
                    _redisTransaction = client.CreateTransaction();
                }
                return _redisTransaction;
            }
        }
    }
}
