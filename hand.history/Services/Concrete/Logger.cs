using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace hand.history.Services.Concrete
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public Logger()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));

            _logger = LogManager.GetLogger(typeof(Logger));
        }

        public void Log(Level level, string message)
        {
            _logger.Debug(message);
        }

        public void Log(Level level, string message, object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);

            _logger.Debug($"message: {message} object:{serialized}");
        }

        public enum Level
        {
            Debug, Warning, Information, Error, Fatal
        }
    }
}
