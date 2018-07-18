using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hand.history.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<TSource> SequentialValues<TSource>(this IEnumerable<TSource> source, int threshold) where TSource : Card
        {
            source = source.OrderBy(element => element.Rank).Distinct();

            var result = new List<TSource>();

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
    }
}
