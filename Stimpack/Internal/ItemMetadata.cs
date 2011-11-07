namespace Stimpack.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Disposables;

    class ItemMetadata
    {
        readonly List<int> indices = new List<int>();
        readonly CompositeDisposable subscriptions = new CompositeDisposable();

        public void AddIndex(int index)
        {
            if (!indices.Contains(index))
            {
                indices.Add(index);
            }
        }

        public IEnumerable<int> Indices
        {
            get { return indices.AsEnumerable(); }
        }

        public void AddSubscription(IDisposable subscription)
        {
            subscriptions.Add(subscription);
        }

        public void DisposeSubscriptions()
        {
            if (!subscriptions.IsDisposed)
            {
                subscriptions.Dispose();
            }
        }
    }
}