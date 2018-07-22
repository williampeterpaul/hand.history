using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.Services.Concrete.Logger;

namespace hand.history.Services
{
    public interface ILogger
    {
        void Log(Level level, string message);
        void Log(Level level, string message, object obj);
    }
}
