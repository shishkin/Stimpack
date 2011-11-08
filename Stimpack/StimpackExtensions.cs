namespace Stimpack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive;
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
        /// Creates a read-only observable collection view with filtering and sorting support.
        /// </summary>
        public static ObservableView<T> CreateView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter = null,
            IComparer<T> order = null)
        {
            return new ObservableView<T>(source, filter, order);
        }

        /// <summary>
        /// Creates an observable for objects implementing INPC.
        /// </summary>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
            this INotifyPropertyChanged target)
        {
            return Observable.FromEventPattern<
                PropertyChangedEventHandler,
                PropertyChangedEventArgs>(
                    x => target.PropertyChanged += x,
                    x => target.PropertyChanged -= x);
        }

        /// <summary>
        /// Creates an observable for a collection implementing INCC.
        /// </summary>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged(
            this IEnumerable source)
        {
            var notifying = source as INotifyCollectionChanged;
            if (notifying == null)
            {
                return Observable.Never<EventPattern<NotifyCollectionChangedEventArgs>>();
            }

            return Observable.FromEventPattern<
                NotifyCollectionChangedEventHandler,
                NotifyCollectionChangedEventArgs>(
                    ev => notifying.CollectionChanged += ev,
                    ev => notifying.CollectionChanged -= ev);
        }

        /// <summary>
        /// Creates an observable for a collection implementing INCC.
        /// </summary>
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedArgs(
            this IEnumerable source)
        {
            return source.ObserveCollectionChanged()
                .Select(x => x.EventArgs);
        }
    }
}
