namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Reactive.Testing;

    using Shouldly;

    using Xunit;

    public class ReactiveSpecs
    {
        readonly TestScheduler scheduler;
        readonly ITestableObserver<int> observer;
        readonly Subject<int> source;
        readonly IEnumerable<int> observedValues;
        readonly IEnumerable<Exception> observedErrors;

        public ReactiveSpecs()
        {
            scheduler = new TestScheduler();
            observer = scheduler.CreateObserver<int>();
            source = new Subject<int>();
            observedValues = observer.Messages
                .Where(x => x.Value.HasValue)
                .Select(x => x.Value.Value);
            observedErrors = observer.Messages
                .Where(x => x.Value.Kind == NotificationKind.OnError)
                .Select(x => x.Value.Exception);
        }

        [Fact]
        public void Has_initial_value()
        {
            var reactive = CreateReactiveAsPowerOfTwo();

            reactive.Value.ShouldBe(0);
        }

        [Fact]
        public void Can_specify_initial_value()
        {
            var reactive = source.ToReactive(3);

            reactive.Value.ShouldBe(3);
        }

        [Fact]
        public void Changes_its_value_upon_notifications()
        {
            var reactive = CreateReactiveAsPowerOfTwo();

            source.OnNext(3);
            source.OnNext(5);

            reactive.Value.ShouldBe(25);
        }

        [Fact]
        public void Notifies_observers_about_value_changes()
        {
            CreateReactiveAsPowerOfTwo();

            source.OnNext(3);
            source.OnNext(5);

            observedValues.ShouldBe(new[] { 9, 25 });
        }

        [Fact]
        public void Can_subscribe_to_multiple_sources()
        {
            var reactive = CreateReactiveAsPowerOfTwo();
            var second = new Subject<int>();
            reactive.SubscribeTo(second);
            var third = new Subject<int>();
            reactive.SubscribeTo(third);

            source.OnNext(2);
            second.OnNext(3);
            third.OnNext(4);
            second.OnNext(5);

            observedValues.ShouldBe(new[] { 4, 3, 4, 5 });
        }

        [Fact]
        public void Ignores_duplicate_value_changes()
        {
            CreateReactiveAsPowerOfTwo();

            source.OnNext(5);
            source.OnNext(5);

            observedValues.ShouldBe(new[] { 25 });
        }

        [Fact]
        public void Provides_component_model_notifications()
        {
            string property = null;
            var reactive = CreateReactiveAsPowerOfTwo();
            reactive.PropertyChanged += (s, e) => { property = e.PropertyName; };

            source.OnNext(5);

            property.ShouldBe("Value");
        }

        [Fact]
        public void Communicates_errors()
        {
            var reactive = source.Select(x => 4 / x).ToReactive();
            reactive.Subscribe(observer);

            source.OnNext(5);
            source.OnNext(0);

            observedErrors.Single().ShouldBeTypeOf<DivideByZeroException>();
        }

        [Fact]
        public void Can_unsubscribe_from_source()
        {
            var reactive = new Reactive<int>();
            reactive.Subscribe(observer);

            using (reactive.SubscribeTo(source))
            {
                source.OnNext(5);
            }

            source.OnNext(10);

            observedValues.ShouldBe(new[] { 5 });
        }

        [Fact]
        public void Active_can_actively_change_its_value()
        {
            var active = source.ToActive();
            active.Subscribe(observer);

            source.OnNext(5);
            active.Value = 8;
            source.OnNext(10);

            observedValues.ShouldBe(new[] { 5, 8, 10 });
        }

        Reactive<int> CreateReactiveAsPowerOfTwo()
        {
            var reactive = source
                .Select(x => x * x)
                .ToReactive();
            reactive.Subscribe(observer);
            return reactive;
        }
    }
}
