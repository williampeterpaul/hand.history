using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IPoller
    {
        bool HasChanges(string path);
        string GetChanges(string path);
    }
}
