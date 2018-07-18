using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IPoller
    {
        void Run();
        string GetChanges();
    }
}
