using Ninject;
using NLog;
using RedisTutorial.Manager;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial
{

    class Program
    {
        public static IKernel kernal { set; get; }
        private static ICustomLogManager _logger;
        public static ICustomLogManager logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = kernal.Get<ICustomLogManager>();
                }
                return _logger;
            }
        }
        private static IRedisManager _redisManager;
        public static IRedisManager redisManager
        {
            get
            {
                if (_redisManager == null)
                {
                    
                    var nativeClient = new Ninject.Parameters.ConstructorArgument("nativeClient", kernal.Get<IRedisNativeClient>());
                    var redisClient = new Ninject.Parameters.ConstructorArgument("redisClient", kernal.Get<IRedisClient>());

                    _redisManager = kernal.Get<IRedisManager>(nativeClient, redisClient);
                }
                return _redisManager;
            }
        }
        static void Main(string[] args)
        {
            try
            {
                //-------------SAMPLE--------------
                //var kernel = new StandardKernel();
                //kernel.Load(Assembly.GetExecutingAssembly());
                //var mailSender = kernel.Get<IMailSender>();
                kernal = new StandardKernel(new DIOperation());

                logger.Debug("Redis Tutorial uygulaması çalıştırıldı");
                logger.Info("set/get işlemleri başladı");
                #region set/get process

                redisManager.SetString("Mesaj:1", "Hello World");
                Console.WriteLine($"Değer : {redisManager.GetString("Mesaj:1")}");

                #endregion
                logger.Info("set/get işlemleri bitti");

                logger.Info("list process işlemleri başladı");
                #region listprocess

                var list = new string[] { "Murat", "Yadigar", "Serkan" };
                redisManager.SetList("names", list);


                foreach (var name in redisManager.GetList("names"))
                    Console.WriteLine($"İsim : {name}");

                #endregion
                logger.Info("list process işlemleri bitti");

                logger.Info("list with model process işlemleri başladı");
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
                logger.Info("list with model process işlemleri bitti");

                logger.Log(LogLevel.Warn, "transaction işlemleri başladı");
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
                logger.Log(LogLevel.Warn, "transaction işlemleri bitti");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Beklenmedik Hata" + ex.Message);
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
