using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Business.Interface
{
    public interface ICustomLogManager
    {
        void Debug(string message);
        void Info(string message);
        void Error(Exception ex, string message);
        void Log(LogLevel level, string message);
    }
}
