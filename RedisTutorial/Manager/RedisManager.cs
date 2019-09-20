using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Manager
{
    public class RedisTypedCreator<T>
    {
        private static IRedisTypedClient<T> _TypedClient;
        private static object locked;

        public static IRedisTypedClient<T> CreateInstance(IRedisClient client)
        {
            locked = "kilitli";
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
    public class RedisTransactionCreator
    {
        private static IRedisTransaction _redisTransaction;
        private static object locked;

        public static IRedisTransaction CreateInstance(IRedisClient client)
        {
            locked = "kilitli";
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
    public class RedisManager : IRedisManager
    {
        //private IRedisNativeClient nativeClient;
        private IRedisClient redisClient;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeClient">basit işlemler stringleri key value işlemleri için kullanılır</param>
        /// <param name="redisClient">Üst seviye işlemler için kullanılır</param>
        public RedisManager(IRedisNativeClient nativeClient, IRedisClient redisClient)
        {
            //this.nativeClient = nativeClient;
            this.redisClient = redisClient;
        }
        #region RedisClient
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
        public void DeleteList(string key)
        {
            redisClient.Lists[key].Clear();
        }
        public T Get<T>(string key) where T : struct
        {
            return redisClient.Get<T>(key);
        }
        #endregion
        #region RedisTypedClient
        public IRedisTypedClient<T> GetRedisTypedClient<T>()
        {
            return RedisTypedCreator<T>.CreateInstance(redisClient);
        }
        public void InsertModel<T>(T model) where T : BaseEntity
        {
            var client = GetRedisTypedClient<T>();
            if (client.GetById(model.Id) == null)
                client.Store(model);
            //else
            //Already stored
        }
        public void InsertModels<T>(List<T> models) where T : BaseEntity
        {
            var client = GetRedisTypedClient<T>();
            var removelist = client.GetByIds(models.Select(i => i.Id));

            var filteredList = models.Where(i => !removelist.Select(j => j.Id).Contains(i.Id));
            if (filteredList.Count() != 0)
                client.StoreAll(filteredList);
            //else
            //Already stored
        }
        public void DeleteAllModels<T>()
        {
            var client = GetRedisTypedClient<T>();
            client.DeleteAll();
        }
        public IList<T> GetAll<T>()
        {
            var client = GetRedisTypedClient<T>();
            return client.GetAll();
        }
        public T GetById<T>(object Id)
        {
            var client = GetRedisTypedClient<T>();
            return client.GetById(Id);
        }
        #endregion
        #region Transaction Process
        public void QueueCommand(Func<IRedisClient, bool> command)
        {
            var client = RedisTransactionCreator.CreateInstance(redisClient);
            client.QueueCommand(command);

        }
        public void QueueCommand(Func<IRedisClient, long> command)
        {
            var client = RedisTransactionCreator.CreateInstance(redisClient);
            client.QueueCommand(command);
        }
        public void Commit()
        {
            var client = RedisTransactionCreator.CreateInstance(redisClient);
            client.Commit();
        }


        #endregion
    }
}
