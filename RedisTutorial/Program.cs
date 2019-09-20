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
                redisManager.DeleteList("names");
                var list = new string[] { "Murat", "Yadigar", "Serkan" };
                redisManager.SetList("names", list);


                foreach (var name in redisManager.GetList("names"))
                    Console.WriteLine($"İsim : {name}");

                #endregion
                logger.Info("list process işlemleri bitti");

                logger.Info("list with model process işlemleri başladı");
                #region list with Model process

                long lastCustomerId = 0;

                redisManager.DeleteAllModels<Order>();
                redisManager.DeleteAllModels<Customer>();

                var order1 = new Order() { Id = Guid.NewGuid(), OrderName = "OrderName_1" };
                var order2 = new Order() { Id = Guid.NewGuid(), OrderName = "OrderName_2" };
                var order3 = new Order() { Id = Guid.NewGuid(), OrderName = "OrderName_3" };
                var order4 = new Order() { Id = Guid.NewGuid(), OrderName = "OrderName_4" };

                var orderList = new List<Order> { order1, order2, order3, order4 };

                redisManager.InsertModels(orderList);




                var customer1 = new Customer()
                {
                    Id = Guid.NewGuid(),
                    Name = "Jess Lewis",
                    Orders = new List<Order>
                {
                   order1,order2
                }
                };

                var customer2 = new Customer()
                {
                    Id = Guid.NewGuid(),
                    Name = "Huseyin Tosun",
                    Orders = new List<Order>
                {
                   order3,order4
                }
                };

                redisManager.InsertModel(customer1);
                redisManager.InsertModel(customer2);
                // GET
                var customer = redisManager.GetById<Customer>(lastCustomerId);

                //Console.WriteLine($"Müşteri ismi : {customer.Name}\nMüşteri Id : {customer.Id}");
                var filteredList = redisManager.GetAll<Customer>().Where(i => i.Name.Contains("1"));
                foreach (var item in redisManager.GetAll<Customer>())
                    Console.WriteLine($"Müşteri ismi : {item.Name}\nMüşteri Id : {item.Id}");
                #endregion
                logger.Info("list with model process işlemleri bitti");

                logger.Log(LogLevel.Warn, "transaction işlemleri başladı");
                #region transaction process

                redisManager.QueueCommand(t => t.Set("Deneme", 1));
                redisManager.QueueCommand(t => t.Increment("Deneme", 1));
                redisManager.QueueCommand(t => t.Increment("Deneme", 1));
                redisManager.QueueCommand(t => t.Increment("Deneme", 1));

                redisManager.Commit();
                var result = redisManager.Get<int>("Deneme");
                Console.WriteLine($"Sonuç : {result}");

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
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order : BaseEntity
    {
        public string OrderName { get; set; }
    }


}
