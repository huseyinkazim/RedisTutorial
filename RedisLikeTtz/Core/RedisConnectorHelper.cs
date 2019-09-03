using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisLikeTtz.Core
{
    public class RedisConnectorHelper
    {

        /// <summary>
        /// Redise bağlantıyı düzenler.
        /// </summary>
        static RedisConnectorHelper()
        {
            if (lazyConnection == null)
            {
                var configOptions = new ConfigurationOptions();
                configOptions.EndPoints.Add("localhost:6379");
                //configOptions.ClientName = "Test1";
                configOptions.ConnectTimeout = 100000;
                configOptions.SyncTimeout = 100000;

                lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(configOptions);
                });
            }
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
