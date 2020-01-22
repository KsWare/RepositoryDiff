using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace KsWare.RepositoryDiff.Common
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach(T item in enumeration) action(item);
        }

        public static void ForEach(this IEnumerable enumeration, Action<object> action)
        {
            foreach(var item in enumeration) action(item);
        }
    }
}
