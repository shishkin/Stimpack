namespace Stimpack.Internal
{
    using System;
    using System.Collections.Generic;

    static class InternalExtensions
    {
        public static int BinarySearch<T>(
            this ICollection<T> collection,
            T value,
            IComparer<T> comparer = null)
        {
            var array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return Array.BinarySearch(array, value, comparer);
        }
    }
}