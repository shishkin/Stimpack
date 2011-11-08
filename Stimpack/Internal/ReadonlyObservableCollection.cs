namespace Stimpack.Internal
{
    using System;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;

    public class ReadonlyObservableCollection<T>
        : ObservableCollection<T>,
        IList<T>,
        IObservable<EventPattern<NotifyCollectionChangedEventArgs>>
    {
        readonly IObservable<EventPattern<NotifyCollectionChangedEventArgs>> notifications;

        protected ReadonlyObservableCollection()
        {
            notifications = this.ObserveCollectionChanged();
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        T IList<T>.this[int index]
        {
            get { return base[index]; }
            set { throw new InvalidOperationException(); }
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public IDisposable Subscribe(
            IObserver<EventPattern<NotifyCollectionChangedEventArgs>> observer)
        {
            return notifications.Subscribe(observer);
        }
    }
}