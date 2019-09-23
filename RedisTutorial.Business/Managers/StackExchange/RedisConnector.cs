using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Managers.StackExchange
{
    public class RedisConnector
    {
        private static ConfigurationOptions configOptions;
        public static object locked = new object();
        static RedisConnector()
        {
            configOptions = new ConfigurationOptions();
            configOptions.EndPoints.Add("localhost:6379");
            //configOptions.ClientName = "ClientName";
            configOptions.ConnectTimeout = 100000;
            configOptions.SyncTimeout = 100000;

        }
        private static ConnectionMultiplexer _connection;


        public static ConnectionMultiplexer TakeInstance()
        {
            lock (locked)
            {
                if (_connection == null)
                    _connection = ConnectionMultiplexer.Connect(configOptions);
                return _connection;
            }
        }
    }
}
