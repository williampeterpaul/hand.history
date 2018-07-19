using hand.history.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Extensions
{
    public static class LoggerExtension
    {
        public static void LogDebug(this ILogger logger, string message) => logger.Log(message);
        public static void LogInformation(this ILogger logger, string message) => logger.Log(message);
        public static void LogWarning(this ILogger logger, string message) => logger.Log(message);
        public static void LogError(this ILogger logger, string message) => logger.Log(message);
        public static void LogFatal(this ILogger logger, string message) => logger.Log(message);
    }
}
