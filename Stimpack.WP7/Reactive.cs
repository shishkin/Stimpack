namespace Stimpack
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    public class Reactive<T> : IObservable<T>, IObserver<T>, INotifyPropertyChanged
    {
        readonly Subject<T> changes = new Subject<T>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Reactive(T initialValue = default(T))
        {
            Value = initialValue;
        }

        public T Value { get; private set; }

        public IDisposable SubscribeTo(IObservable<T> source)
        {
            return source.Subscribe(this);
        }

        void IObserver<T>.OnNext(T next)
        {
            if (Equals(Value, next))
                return;

            Value = next;
            changes.OnNext(next);
            PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }

        void IObserver<T>.OnError(Exception exception)
        {
            changes.OnError(exception);
        }

        void IObserver<T>.OnCompleted()
        {
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return changes.Subscribe(observer);
        }
    }
}