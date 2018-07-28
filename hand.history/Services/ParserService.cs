using hand.history.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace hand.history.Services
{
    public sealed class ParserService : IParser
    {
        public string ParseString(string value, string pattern) => Parse<string>(value, pattern);
        public double ParseDouble(string value, string pattern) => Parse<double>(value, pattern);
        public int ParseInteger(string value, string pattern) => Parse<int>(value, pattern);

        public IEnumerable<string> ParseStringMulti(string value, string pattern) => ParseMulti<string>(value, pattern);
        public IEnumerable<double> ParseDoubleMulti(string value, string pattern) => ParseMulti<double>(value, pattern);
        public IEnumerable<int> ParseIntegerMulti(string value, string pattern) => ParseMulti<int>(value, pattern);

        public Type Parse<Type>(string value, string pattern)
        {
            var result = Regex.Match(value, pattern).Value;
            return (Type)Convert.ChangeType(value, typeof(Type));
        }

        public IEnumerable<Type> ParseMulti<Type>(string value, string pattern)
        {
            foreach (Match item in Regex.Matches(value, pattern))
            {
                var result = item.Value;
                yield return ((Type)Convert.ChangeType(result, typeof(Type)));
            }
        }

        public DateTime ParseDateTime(string value, string pattern)
        {
            var result = Parse<string>(value, pattern);
            return DateTime.Parse(result);
        }
    }
}
