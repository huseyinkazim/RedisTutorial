using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Managers.ServiceStack
{
    public class RedisTypedCreator<T>
    {
        private static IRedisTypedClient<T> _TypedClient;
        private static object locked=new object();

        public static IRedisTypedClient<T> CreateInstance(IRedisClient client)
        {
            lock (locked)
            {
                if (_TypedClient == null)
                {
                    _TypedClient = client.As<T>();
                }
                return _TypedClient;
            }
        }
    }
}
