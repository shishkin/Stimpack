namespace Stimpack.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

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

        public static PropertyInfo GetPropertyInfo(this LambdaExpression property)
        {
            return Enumerable.Repeat(property, 1)
                .Select(x => x.Body)
                .OfType<MemberExpression>()
                .Select(x => x.Member)
                .OfType<PropertyInfo>()
                .FirstOrDefault();
        }
    }
}