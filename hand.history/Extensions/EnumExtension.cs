using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hand.history.Extensions
{
    public static class EnumExtension
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static object TryParseDefault<T>(string value)
        {
            object result = default(T);

            if (Enum.IsDefined(typeof(T), value))
            {
                result = Enum.Parse(typeof(T), value, true);
            }

            return result;
        }
    }
}
