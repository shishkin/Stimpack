namespace Stimpack.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

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

        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedArgs(
            this IEnumerable source)
        {
            var notifying = source as INotifyCollectionChanged;
            if (notifying == null)
            {
                return Observable.Never<NotifyCollectionChangedEventArgs>();
            }

            return Observable.FromEventPattern<
                NotifyCollectionChangedEventHandler,
                NotifyCollectionChangedEventArgs>(
                    ev => notifying.CollectionChanged += ev,
                    ev => notifying.CollectionChanged -= ev)
                .Select(x => x.EventArgs);
        }
    }
}