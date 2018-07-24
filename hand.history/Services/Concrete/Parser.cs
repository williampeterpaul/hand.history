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

        public IEnumerable<string> ParseStringMulti(string value, string pattern) => ParseMulti<string>(value, pattern);

        public IEnumerable<double> ParseDoubleMulti(string value, string pattern) => ParseMulti<double>(value, pattern);

        public IEnumerable<int> ParseIntegerMulti(string value, string pattern) => ParseMulti<int>(value, pattern);

        private Type Parse<Type>(string value, string pattern)
        {
            var result = Regex.Match(value, pattern).Value;

            if (string.IsNullOrWhiteSpace(result)) return default(Type);

            return (Type)Convert.ChangeType(result, typeof(Type));
        }

        private IEnumerable<Type> ParseMulti<Type>(string value, string pattern)
        {
            var matches = Regex.Matches(value, pattern);
            var result = new List<Type>();

            foreach (Match item in matches)
            {
                if (string.IsNullOrWhiteSpace(item.Value)) continue;

                result.Add((Type)Convert.ChangeType(item.Value, typeof(Type)));
            }

            return result;
        }

        public DateTime ParseDateTime(string value, string pattern)
        {
            var result = Parse<string>(value, pattern);

            if (string.IsNullOrWhiteSpace(result)) return DateTime.Now;

            return DateTime.Parse(result);
        }
    }
}
