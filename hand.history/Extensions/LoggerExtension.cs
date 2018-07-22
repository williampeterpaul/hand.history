using hand.history.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.Services.Concrete.Logger;

namespace hand.history.Extensions
{
    public static class LoggerExtension
    {
        public static void LogDebug(this ILogger logger, string message) => logger.Log(Level.Debug, message);
        public static void LogDebug(this ILogger logger, string message, object obj) => logger.Log(Level.Debug, message, obj);

        public static void LogInformation(this ILogger logger, string message) => logger.Log(Level.Information, message);
        public static void LogInformation(this ILogger logger, string message, object obj) => logger.Log(Level.Information, message, obj);

        public static void LogWarning(this ILogger logger, string message) => logger.Log(Level.Warning, message);
        public static void LogWarning(this ILogger logger, string message, object obj) => logger.Log(Level.Warning, message, obj);

        public static void LogError(this ILogger logger, string message) => logger.Log(Level.Error, message);
        public static void LogError(this ILogger logger, string message, object obj) => logger.Log(Level.Error, message, obj);

        public static void LogFatal(this ILogger logger, string message) => logger.Log(Level.Fatal, message);
        public static void LogFatal(this ILogger logger, string message, object obj) => logger.Log(Level.Fatal, message, obj);

    }
}
