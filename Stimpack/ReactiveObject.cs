namespace Stimpack
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Internal;

    public class ReactiveObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void React()
        {
            GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead)
                .Where(x => typeof(INotifyPropertyChanged).IsAssignableFrom(x.PropertyType))
                .ToDictionary(x => x.Name, x => x.GetValue(this, null) as INotifyPropertyChanged)
                .Where(x => x.Value != null)
                .ForEach(x => x.Value
                    .ObservePropertyChanged()
                    .Subscribe(_ => OnPropertyChanged(x.Key)));
        }

        protected void Upon<TSource, TResult>(
            IObservable<TSource> changes,
            Expression<Func<TResult>> property,
            Func<TSource, TResult> selector)
        {
            changes.Subscribe(value => Update(property, selector(value)));
            var reactive = changes as Reactive<TSource>;
            if (reactive != null)
            {
                Update(property, selector(reactive.Value));
            }
        }

        void Update<T>(Expression<Func<T>> property, T value)
        {
            Enumerable.Repeat(property, 1)
                .Select(x => x.GetPropertyInfo())
                .Where(x => x.CanWrite)
                .ToList()
                .ForEach(x => SetProperty(x, value));
        }

        void SetProperty<T>(PropertyInfo property, T value)
        {
            var old = property.GetValue(this, null);
            if (Equals(old, value))
            {
                return;
            }

            property.SetValue(this,
                value,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.SetProperty,
                null,
                null,
                null);
            OnPropertyChanged(property.Name);
        }
    }
}