namespace Stimpack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    public static class StimpackExtensions
    {
        /// <summary>
        /// Creates a reactive value based on the observable sequence.
        /// </summary>
        public static Reactive<T> ToReactive<T>(
            this IObservable<T> source,
            T initialValue = default(T))
        {
            var value = new Reactive<T>(initialValue);
            value.SubscribeTo(source);
            return value;
        }

        /// <summary>
        /// Creates a reactive value based on the observable sequence.
        /// </summary>
        public static Active<T> ToActive<T>(
            this IObservable<T> source,
            T initialValue = default(T))
        {
            var value = new Active<T>(initialValue);
            value.SubscribeTo(source);
            return value;
        }

        /// <summary>
        /// Creates a read-only observable transformation of the source using the selector.
        /// </summary>
        public static Transformation<TSource, TResult> Transform<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return new Transformation<TSource, TResult>(source, selector);
        }

        /// <summary>
        /// Returns an observable sequence of the source collection change notifications.
        /// Returns Observable.Never for collections not implementing INCC.
        /// </summary>
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
