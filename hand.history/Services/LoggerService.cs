
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using hand.history.Services.Interfaces;
using log4net;
using log4net.Config;

namespace hand.history.Services
{
    public class LoggerService : ILogger
    {
        private ILog Logger { get; }

        public LoggerService()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));

            Logger = LogManager.GetLogger(typeof(LoggerService));
        }

        public LoggerService(string configuration)
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(configuration));

            Logger = LogManager.GetLogger(typeof(LoggerService));
        }

        public void Log(Level level, string message, object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);

            Log(level, $"message: {message} object:{serialized}");
        }

        public void Log(Level level, string message)
        {
            switch (level)
            {
                case Level.Debug: Logger.Debug(message); break;
                case Level.Information: Logger.Info(message); break;
                case Level.Warning: Logger.Warn(message); break;
                case Level.Error: Logger.Error(message); break;
                case Level.Fatal: Logger.Fatal(message); break;
            }
        }

        public enum Level
        {
            Debug, Warning, Information, Error, Fatal
        }
    }
}
