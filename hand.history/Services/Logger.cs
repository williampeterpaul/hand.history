
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
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public Logger()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));

            _logger = LogManager.GetLogger(typeof(Logger));
        }

        public Logger(string configuration)
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(configuration));

            _logger = LogManager.GetLogger(typeof(Logger));
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
                case Level.Debug: _logger.Debug(message); break;
                case Level.Information: _logger.Info(message); break;
                case Level.Warning: _logger.Warn(message); break;
                case Level.Error: _logger.Error(message); break;
                case Level.Fatal: _logger.Fatal(message); break;
            }
        }

        public enum Level
        {
            Debug, Warning, Information, Error, Fatal
        }
    }
}
