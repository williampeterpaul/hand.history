using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IParser
    {
        string ParseString();
        string ParseDouble();
        string ParseInteger();
    }
}
