using hand.history.DataObject;
using hand.history.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Extensions
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
