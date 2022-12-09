using System;
using System.Collections.Generic;

namespace LSlicer.Helpers
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action) 
        {
            foreach (var item in collection)
            {
                action.Invoke(item);
            }
            return collection;
        }
    }
}
