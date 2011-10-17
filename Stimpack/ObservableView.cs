namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;

    using Internal;

    public class ObservableView<T> : ReadonlyObservableCollection<T>
    {
        readonly IEnumerable<T> source;
        readonly Func<T, bool> filter;
        readonly IComparer<T> order;

        public ObservableView(
            IEnumerable<T> source,
            Func<T, bool> filter = null,
            IComparer<T> order = null)
        {
            this.source = source ?? Enumerable.Empty<T>();
            this.filter = filter ?? (_ => true);
            this.order = order;

            FetchItems();

            BindNotifications();
        }

        void FetchItems()
        {
            ClearItems();

            var items = source.Where(filter);
            if (order != null)
            {
                items = items.OrderBy(_ => _, order);
            }

            foreach (var item in items)
            {
                InsertItem(Count, item);
            }
        }

        void BindNotifications()
        {
            var changes = source.ObserveCollectionChangedArgs();

            changes.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(_ => FetchItems());

            changes.Where(HasItemsToRemove)
                .SelectMany(x => x.OldItems.Cast<T>())
                .Subscribe(OnItemToRemove);

            changes.Where(HasItemsToAdd)
                .SelectMany(x => x.NewItems
                    .Cast<T>()
                    .Select(value => new { x.NewStartingIndex, value }))
                .Subscribe(x => OnItemToAdd(x.NewStartingIndex, x.value));
        }

        void OnItemToAdd(int index, T value)
        {
            if (!filter(value))
                return;

            InsertItem(GetNewIndex(index, value), value);
        }

        void OnItemToRemove(T value)
        {
            var index = Items.IndexOf(value);
            if (index >= 0)
            {
                RemoveItem(index);
            }
        }

        int GetNewIndex(int sourceIndex, T value)
        {
            if (order == null)
            {
                return Math.Min(Math.Max(0, sourceIndex), Count);
            }

            var match = Items.BinarySearch(value, order);
            return match < 0 ? Math.Abs(match + 1) : match;
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