namespace Stimpack.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
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

        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
        this INotifyPropertyChanged target)
        {
            return Observable.FromEventPattern<
                PropertyChangedEventHandler,
                PropertyChangedEventArgs>(
                    x => target.PropertyChanged += x,
                    x => target.PropertyChanged -= x)
                .Select(x => x);
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