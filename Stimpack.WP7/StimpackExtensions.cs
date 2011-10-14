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
    }
}
