namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;

    using Internal;

    public class ObservableView<T> : ReadonlyObservableCollection<T>
    {
        readonly Dictionary<T, ItemMetadata> tracks =
            new Dictionary<T, ItemMetadata>();
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

            RebuildFull();

            BindNotifications();
        }

        void RebuildFull()
        {
            ClearAll();

            source.ForEach((item, index) => OnItemToAdd(index, item));
        }

        void ClearAll()
        {
            ClearItems();
            tracks.Values.ForEach(x => x.DisposeSubscriptions());
            tracks.Clear();
        }

        void BindNotifications()
        {
            var changes = source.ObserveCollectionChangedArgs();

            changes.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(_ => RebuildFull());

            changes.Where(HasItemsToRemove)
                .SelectMany(x => x.OldItems.Cast<T>())
                .Subscribe(OnItemToRemove);

            changes.Where(HasItemsToAdd)
                .SelectMany(x => x.NewItems
                    .Cast<T>()
                    .Select(value => new { x.NewStartingIndex, value }))
                .Subscribe(x => OnItemToAdd(x.NewStartingIndex, x.value));
        }

        void OnItemToAdd(int index, T item)
        {
            TrackItem(item, index);

            if (!filter(item))
                return;

            InsertItem(GetNewIndex(index, item), item);
        }

        void OnItemToRemove(T item)
        {
            UntrackItem(item);

            var index = Items.IndexOf(item);
            if (index >= 0)
            {
                RemoveItem(index);
            }
        }

        void TrackItem(T item, int index)
        {
            if (!(item is INotifyPropertyChanged))
            {
                return;
            }

            ItemMetadata track;
            if (!tracks.TryGetValue(item, out track))
            {
                tracks[item] = track = new ItemMetadata();
                var subscription = SubscribeItemChanges((INotifyPropertyChanged)item);
                track.AddSubscription(subscription);
            }

            track.AddIndex(index);
        }

        IDisposable SubscribeItemChanges(INotifyPropertyChanged item)
        {
            return item.ObservePropertyChanged()
                .Subscribe(x => OnItemChanged((T)x.Sender));
        }

        void OnItemChanged(T item)
        {
            var contains = Contains(item);
            var shouldContain = filter(item);

            if (contains && !shouldContain)
            {
                OnItemToRemove(item);
            }

            if (!contains && shouldContain)
            {
                tracks[item].Indices.ForEach(index => OnItemToAdd(index, item));
            }
        }

        void UntrackItem(T item)
        {
            ItemMetadata track;
            if (!tracks.TryGetValue(item, out track))
            {
                return;
            }

            track.DisposeSubscriptions();
            tracks.Remove(item);
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