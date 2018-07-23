using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace hand.history.Services.Concrete
{
    public sealed class Parser : IParser
    {
        public string ParseString(string value, string pattern) => Parse<string>(value, pattern);

        public double ParseDouble(string value, string pattern) => Parse<double>(value, pattern);

        public int ParseInteger(string value, string pattern) => Parse<int>(value, pattern);

        public DateTime ParseDateTime(string value, string pattern) => DateTime.Parse(Parse<string>(value, pattern));

        private Type Parse<Type>(string value, string pattern)
        {
            var result = Regex.Match(value, pattern).Value;

            if (string.IsNullOrWhiteSpace(result)) return default(Type);

            return (Type) Convert.ChangeType(result, typeof(Type));
        }

    }
}
