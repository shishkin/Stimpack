namespace Stimpack
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ReactiveObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
                .Select(x => x.Body)
                .OfType<MemberExpression>()
                .Select(x => x.Member)
                .OfType<PropertyInfo>()
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
            PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
        }
    }
}