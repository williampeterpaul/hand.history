using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.Services.LoggerService;

namespace hand.history.Services.Interfaces
{
    public interface ILogger
    {
        void Log(Level level, string message);
        void Log(Level level, string message, object obj);
    }
}
