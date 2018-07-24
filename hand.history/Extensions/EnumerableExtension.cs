using hand.history.Models;
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
    }
}
