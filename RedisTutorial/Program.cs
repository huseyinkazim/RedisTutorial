using Ninject;
using NLog;
using RedisTutorial.Business.Interface;
using RedisTutorial.Business.Managers.StackExchange;
using RedisTutorial.Data.Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedisTutorial
{

    class Program
    {
        public static IKernel kernal { set; get; }
        private static IRedisManagerWithServiceStack redisManager;
        private static ICustomLogManager logger;
        private static RedisManagerWithStackExchange stackExchange;

        static void Main(string[] args)
        {
            try
            {
                //-------------SAMPLE--------------
                //var kernel = new StandardKernel();
                //kernel.Load(Assembly.GetExecutingAssembly());
                //var mailSender = kernel.Get<IMailSender>();
                kernal = new StandardKernel(new DIOperation());
                Initialize();
                logger.Debug("Redis Tutorial uygulaması çalıştırıldı");

                #region RedisWithStackExchange
                logger.Info("InsertItem işlemleri başladı");
                #region insertList
                stackExchange.InsertItem("order1", new Order() { Id = Guid.NewGuid(), OrderName = "OrderName_1" });
                #endregion
                logger.Info("InsertItem işlemleri bitti");

                logger.Info("GetListItemRange işlemleri başladı");
                #region GetListItemRange
                var orderlist = stackExchange.GetListItemRange<Order>("order1");
                #endregion
                logger.Info("GetListItemRange işlemleri bitti");

                logger.Info("RemoveList işlemleri başladı");
                #region RemoveList
                stackExchange.RemoveList("order1");
                #endregion
                logger.Info("RemoveList işlemleri bitti");

                #endregion
                #region RedisWithServiceStack
                logger.Info("set/get işlemleri başladı");
                #region set/get process

                redisManager.SetString("Mesaj:1", "Hello World");
                Console.WriteLine($"Değer : {redisManager.GetString("Mesaj:1")}");

                #endregion
                logger.Info("set/get işlemleri bitti");

                logger.Info("list process1 işlemleri başladı");
                #region listprocess1
                redisManager.DeleteList("names");
                var list1 = new string[] { "Murat", "Yadigar", "Serkan" };
                redisManager.SetList("names", list1);


                foreach (var name in redisManager.GetList("names"))
                    Console.WriteLine($"İsim : {name}");

                #endregion
                logger.Info("list process1 işlemleri bitti");

                logger.Info("list process2 işlemleri başladı");
                #region listprocess2
                redisManager.DeleteList("orderTest");
                var list2 = new Order[] { new Order { Id = Guid.NewGuid(), OrderName = "OrderName_1" } };

                redisManager.SetList("orderTest", list2);
                var models = redisManager.GetList<Order>("orderTest");

                foreach (var name in redisManager.GetList("orderTest"))
                    Console.WriteLine($"İsim : {name}");

                #endregion
                logger.Info("list process2 işlemleri bitti");

                logger.Info("list with model process işlemleri başladı");
                #region list with Model process

                long lastCustomerId = 0;

                // redisManager.DeleteAllModels<Order>();
                redisManager.DeleteAllModels<Customer>();
                var order_Id1 = Guid.NewGuid();
                var order_Id2 = Guid.NewGuid();
                var order_Id3 = Guid.NewGuid();
                var order_Id4 = Guid.NewGuid();
                IEnumerable<object> orderIdList = new List<object> { order_Id2, order_Id3, order_Id4 };

                var order1 = new Order() { Id = order_Id1, OrderName = "OrderName_1" };
                var order2 = new Order() { Id = order_Id2, OrderName = "OrderName_2" };
                var order3 = new Order() { Id = order_Id3, OrderName = "OrderName_3" };
                var order4 = new Order() { Id = order_Id4, OrderName = "OrderName_4" };

                var orderList = new List<Order> { order1, order2, order3, order4 };

                redisManager.DeleteById<Order>(order_Id1);
                redisManager.DeleteByIds<Order>(orderIdList);

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
                #endregion
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
        static void Initialize()
        {
            if (logger == null)
            {
                logger = kernal.Get<ICustomLogManager>();
            }
            if (redisManager == null)
            {

                var nativeClient = new Ninject.Parameters.ConstructorArgument("nativeClient", kernal.Get<IRedisNativeClient>());
                var redisClient = new Ninject.Parameters.ConstructorArgument("redisClient", kernal.Get<IRedisClient>());

                redisManager = kernal.Get<IRedisManagerWithServiceStack>(nativeClient, redisClient);
            }
            if (stackExchange == null)
            {
                stackExchange = new RedisManagerWithStackExchange();
            }

        }
    }



}
