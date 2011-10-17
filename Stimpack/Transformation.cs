namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;

    using Internal;

    public class Transformation<TSource, TResult> : ReadonlyObservableCollection<TResult>
    {
        readonly IEnumerable<TSource> source;
        readonly Func<TSource, TResult> selector;

        public Transformation(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            this.source = source;
            this.selector = selector;

            FetchValues();

            BindNotifications();
        }

        void FetchValues()
        {
            ClearItems();

            foreach (var item in source.Select(selector))
            {
                InsertItem(Count, item);
            }
        }

        void BindNotifications()
        {
            var changes = source.ObserveCollectionChangedArgs();

            changes.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(_ => FetchValues());

            changes.Where(HasItemsToRemove)
                .Select(x => x.OldStartingIndex)
                .Subscribe(RemoveItem);

            changes.Where(HasItemsToAdd)
                .SelectMany(x => x.NewItems
                    .Cast<TSource>()
                    .Select(value => new { x.NewStartingIndex, value }))
                .Subscribe(x => InsertItem(x.NewStartingIndex, selector(x.value)));
        }

        static bool HasItemsToAdd(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Add ||
                notification.Action == NotifyCollectionChangedAction.Replace;
        }

        static bool HasItemsToRemove(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Remove ||
                notification.Action == NotifyCollectionChangedAction.Replace;
        }
    }
}