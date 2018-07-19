using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IParser
    {
        string ParseString(string value, string pattern);
        double ParseDouble(string value, string pattern);
        int ParseInteger(string value, string pattern);
        DateTime ParseDateTime(string value, string pattern);
    }
}
