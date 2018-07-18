using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace hand.history.Services.Concrete
{
    public class RegexParser : IParser
    {
        public string ParseString(string value, string pattern) => Parse<string>(value, pattern);

        public double ParseDouble(string value, string pattern) => Parse<double>(value, pattern);

        public int ParseInteger(string value, string pattern) => Parse<int>(value, pattern);

        private Type Parse<Type>(string value, string pattern)
        {
            var result = Regex.Match(value, pattern).Value;

            return (Type) Convert.ChangeType(result, typeof(Type));
        }

    }
}
