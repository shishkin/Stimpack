namespace Stimpack
{
    using System;

    public static class StimpackExtensions
    {
        /// <summary>
        /// Creates a reactive value based on the observable sequence.
        /// </summary>
        public static Reactive<T> ToReactive<T>(
            this IObservable<T> source,
            T defaultValue = default(T))
        {
            var value = new Reactive<T>(defaultValue);
            value.SubscribeTo(source);
            return value;
        }
    }
}
