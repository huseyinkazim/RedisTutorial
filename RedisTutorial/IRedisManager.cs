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
        void SetList(string key, string[] list);
        List<string> GetList(string key);
    }
}
