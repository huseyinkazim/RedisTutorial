using NLog;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial
{
    public class CustomLogManager
    {
        private Logger _logger;
        private static object locked = new object();
        public CustomLogManager()
        {
            System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        public Logger logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetCurrentClassLogger();
                }
                return _logger;
            }
        }
        public void Debug(string message) => logger.Debug(message);
        public void Info(string message) => logger.Info(message);
        public void Error(Exception ex, string message) => logger.Error(ex, message);
        public void Log(LogLevel level, string message) => logger.Log(level, message);
    }
    class Program
    {
        private static CustomLogManager _logger = new CustomLogManager();
        static void Main(string[] args)
        {
            try
            {
                _logger.Debug("Redis Tutorial uygulaması çalıştırıldı");
                _logger.Info("set/get işlemleri başladı");
                #region set/get process
                using (IRedisNativeClient client = new RedisClient())
                {
                    client.Set("Mesaj:1", Encoding.UTF8.GetBytes("Hello World"));

                    string result = Encoding.UTF8.GetString(client.Get("Mesaj:1"));
                    Console.WriteLine($"Değer : {result}");
                }
                #endregion
                _logger.Info("set/get işlemleri bitti");

                _logger.Info("list process işlemleri başladı");
                #region listprocess

                using (IRedisClient client = new RedisClient())
                {
                    var names = client.Lists["names"];
                    names.Clear();

                    names.Add("Murat");
                    names.Add("Yadigar");
                    names.Add("Serkan");
                }

                using (IRedisClient client = new RedisClient())
                {
                    var names = client.Lists["names"];

                    foreach (var name in names)
                        Console.WriteLine($"İsim : {name}");
                }
                #endregion
                _logger.Info("list process işlemleri bitti");

                _logger.Info("list with model process işlemleri başladı");
                #region list with Model process

                long lastCustomerId = 0;

                // SET
                using (IRedisClient client = new RedisClient())
                {

                    //IRedisTypedClient
                    var customerClient = client.As<Customer>();
                    customerClient.DeleteAll();

                    var customer1 = new Customer()
                    {
                        Id = customerClient.GetNextSequence(),
                        Name = "Jess Lewis",
                        Orders = new List<Order>
                {
                   new Order() { Id = 1 },
                   new Order() { Id = 2 }
                }
                    };
                    var customer2 = new Customer()
                    {
                        Id = customerClient.GetNextSequence(),
                        Name = "Huseyin Tosun",
                        Orders = new List<Order>
                {
                   new Order() { Id = 3 },
                   new Order() { Id = 4 }
                }
                    };

                    var savedCustomer1 = customerClient.Store(customer1);
                    var savedCustomer2 = customerClient.Store(customer2);
                    lastCustomerId = savedCustomer1.Id;
                }

                // GET
                using (IRedisClient client = new RedisClient())
                {
                    var customerClient = client.As<Customer>();
                    var customer = customerClient.GetById(lastCustomerId);

                    //Console.WriteLine($"Müşteri ismi : {customer.Name}\nMüşteri Id : {customer.Id}");

                    foreach (var item in customerClient.GetAll())
                        Console.WriteLine($"Müşteri ismi : {item.Name}\nMüşteri Id : {item.Id}");


                }
                #endregion
                _logger.Info("list with model process işlemleri bitti");

                _logger.Log(LogLevel.Warn, "transaction işlemleri başladı");
                #region transaction process
                using (IRedisClient client = new RedisClient())
                {
                    var transaction = client.CreateTransaction();
                    transaction.QueueCommand(t => t.Set("Deneme", 1));
                    transaction.QueueCommand(t => t.Increment("Deneme", 1));
                    transaction.Commit();

                    var result = client.Get<int>("Deneme");

                    Console.WriteLine($"Sonuç : {result}");
                }
                #endregion
                _logger.Log(LogLevel.Warn, "transaction işlemleri bitti");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Beklenmedik Hata" + ex.Message);
            }

            finally
            {
                Console.ReadKey();
            }
        }
    }


    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public long Id { get; set; }
    }


}
