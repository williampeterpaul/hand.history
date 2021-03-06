﻿using hand.history.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hand.history.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> SequentialValues<T>(this IEnumerable<T> source, int threshold) where T : Card
        {
            source = source.OrderBy(element => element.Rank).Distinct();

            var result = new List<T>();

            var last = source.First();

            foreach (var element in source.Skip(1))
            {
                if (element.Rank == last.Rank + 1) result.Add(element);
                else result.Clear();

                last = element;

                if (result.Count() == threshold - 1) return result;
            }

            return result;
        }

        public static IEnumerable<int> FindIndexes<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            int index = 0;

            foreach (T item in items)
            {
                if (predicate(item))
                {
                    yield return index;
                }

                index++;
            }
        }

        public static IEnumerable<T2> ConvertType<T1, T2>(this IEnumerable<T1> items, Func<T1, T2> converter)
        {
            foreach (T1 item in items)
            {
                yield return converter(item);
            }
        }
    }
}
