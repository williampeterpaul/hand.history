using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services.Interfaces
{
    public interface IParser
    {
        string ParseString(string value, string pattern);
        double ParseDouble(string value, string pattern);
        decimal ParseDecimal(string value, string pattern);
        int ParseInteger(string value, string pattern);

        IEnumerable<string> ParseStringMulti(string value, string pattern);
        IEnumerable<double> ParseDoubleMulti(string value, string pattern);
        IEnumerable<decimal> ParseDecimalMulti(string value, string pattern);
        IEnumerable<int> ParseIntegerMulti(string value, string pattern);

        DateTime ParseDateTime(string value, string pattern);
    }
}
