using Newtonsoft.Json;
using RedisTutorial.Business.Interface;
using RedisTutorial.Data.Model;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Managers.ServiceStack
{
    public class RedisManagerWithServiceStack : IRedisManagerWithServiceStack
    {
        private readonly IRedisClient redisClient;
        private IRedisTransaction _redisTransaction => RedisTransactionCreator.CreateInstance(redisClient);
        private T SToObject<T>(string value) => JsonConvert.DeserializeObject<T>(value);
        private string OToString<T>(T value) => JsonConvert.SerializeObject(value);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Üst seviye işlemler için kullanılır</param>
        public RedisManagerWithServiceStack(IRedisClient redisClient)
        {
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
        public void SetList<T>(string key, T[] list)
        {
            var names = redisClient.Lists[key];
            for (int i = 0; i < list.Length; i++)
                names.Add(OToString(list[i]));
        }
        public List<string> GetList(string key)
        {
            var names = redisClient.Lists[key].ToList();
            return names;
        }
        public List<T> GetList<T>(string key) where T : class
        {
            var names = redisClient.Lists[key].ToArray();
            var obj = new List<T>(Array.ConvertAll(names, value => SToObject<T>(value)).ToList());
            return obj;
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
        public void DeleteById<T>(object id)
        {
            var client = GetRedisTypedClient<T>();
            client.DeleteById(id);
        }
        public void DeleteByIds<T>(IEnumerable<object> ids)
        {
            var client = GetRedisTypedClient<T>();
            client.DeleteByIds(ids);
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
           // _redisTransaction = RedisTransactionCreator.CreateInstance(redisClient);
            _redisTransaction.QueueCommand(command);
        }
        public void QueueCommand(Func<IRedisClient, long> command)
        {
           // _redisTransaction = RedisTransactionCreator.CreateInstance(redisClient);
            _redisTransaction.QueueCommand(command);
        }
        public void Commit()
        {
           // _redisTransaction = RedisTransactionCreator.CreateInstance(redisClient);
            _redisTransaction.Commit();
        }


        #endregion
    }
}
