using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial
{
    public interface IRedisManager
    {
        void SetString(string key, string value);
        string GetString(string key);
        T Get<T>(string key) where T : struct;
        void SetList(string key, string[] list);
        List<string> GetList(string key);
        void DeleteList(string key);
        void InsertModel<T>(T model) where T : BaseEntity;
        void InsertModels<T>(List<T> models) where T : BaseEntity;
        void DeleteAllModels<T>();
        IList<T> GetAll<T>();
        T GetById<T>(object Id);
        void QueueCommand(Func<IRedisClient, bool> command);
        void QueueCommand(Func<IRedisClient, long> command);
        void Commit();
    }
}
