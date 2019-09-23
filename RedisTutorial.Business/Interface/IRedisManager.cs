using RedisTutorial.Data.Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Interface
{
    public interface IRedisManagerWithServiceStack
    {
        void SetString(string key, string value);
        string GetString(string key);
        T Get<T>(string key) where T : struct;
        void SetList(string key, string[] list);
        void SetList<T>(string key, T[] list);
        List<string> GetList(string key);
        List<T> GetList<T>(string key) where T : class;
        void DeleteList(string key);
        void InsertModel<T>(T model) where T : BaseEntity;
        void InsertModels<T>(List<T> models) where T : BaseEntity;
        void DeleteAllModels<T>();
        IList<T> GetAll<T>();
        T GetById<T>(object Id);
        void QueueCommand(Func<IRedisClient, bool> command);
        void QueueCommand(Func<IRedisClient, long> command);
        void Commit();
        void DeleteById<T>(object id);
        void DeleteByIds<T>(IEnumerable<object> ids);
    }
}
