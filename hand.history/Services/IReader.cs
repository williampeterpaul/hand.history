using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IReader
    {
        string Read(string path);
    }
}
