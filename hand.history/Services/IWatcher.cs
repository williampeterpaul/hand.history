using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IWatcher
    {
        void Run(string path);
        string GetChanges(string path);
    }
}
