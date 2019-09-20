using Ninject;
using NLog;
using System;

namespace RedisTutorial.Manager
{
    public class CustomLogManager: ICustomLogManager
    {
        private ILogger logger;
        private static object locked = new object();
        public CustomLogManager()
        {
            System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            logger = LogManager.GetCurrentClassLogger();
        }

    
        public void Debug(string message) => logger.Debug(message);
        public void Info(string message) => logger.Info(message);
        public void Error(Exception ex, string message) => logger.Error(ex, message);
        public void Log(LogLevel level, string message) => logger.Log(level, message);
    }
}
