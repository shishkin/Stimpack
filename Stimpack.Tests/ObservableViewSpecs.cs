namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Shouldly;

    using Xunit;

    public class ObservableViewSpecs
    {
        readonly ObservableCollection<int> source;

        public ObservableViewSpecs()
        {
            source = new ObservableCollection<int> { 5, 4, 3, 2, 1 };
        }

        [Fact]
        public void View_is_readonly()
        {
            IList<int> trans = new ObservableView<int>(source);

            trans.IsReadOnly.ShouldBe(true);
            Should.Throw<InvalidOperationException>(() => trans.Add(3));
            Should.Throw<InvalidOperationException>(() => trans.Clear());
            Should.Throw<InvalidOperationException>(() => trans.Remove(3));
            Should.Throw<InvalidOperationException>(() => trans.RemoveAt(0));
            Should.Throw<InvalidOperationException>(() => trans.Insert(0, 3));
            Should.Throw<InvalidOperationException>(() => trans[0] = 3);
        }

        [Fact]
        public void Fetches_items_from_source()
        {
            var subject = new ObservableView<int>(source);

            subject.ShouldBe(source);
        }

        [Fact]
        public void Filters_items()
        {
            var subject = new ObservableView<int>(source, x => x % 2 == 0);

            subject.ToArray().ShouldBe(new[] { 4, 2 });
        }

        [Fact]
        public void Sorts_items()
        {
            var subject = new ObservableView<int>(source, null, Comparer<int>.Default);

            subject.ToArray().ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void Adds_items_upon_notification()
        {
            var subject = new ObservableView<int>(source);

            source.Add(10);
            source.Insert(0, 11);
            source.Insert(3, 12);

            subject.ToArray().ShouldBe(new[] { 11, 5, 4, 12, 3, 2, 1, 10 });
        }

        [Fact]
        public void Filters_newly_added_items()
        {
            var subject = new ObservableView<int>(source, x => x % 2 == 0);

            source.Add(10);
            source.Add(11);
            source.Insert(0, 12);
            source.Insert(0, 13);
            source.Insert(1, 14);

            subject.ToArray().ShouldBe(new[] { 12, 14, 4, 2, 10 });
        }

        [Fact]
        public void New_items_preserve_sort_order()
        {
            var subject = new ObservableView<int>(source, null, Comparer<int>.Default);

            source.Add(-2);
            source.Add(3);
            source.Add(3);
            source.Add(10);

            subject.ToArray().ShouldBe(new[] { -2, 1, 2, 3, 3, 3, 4, 5, 10 });
        }

        [Fact]
        public void Removes_values_upon_notification()
        {
            var subject = new ObservableView<int>(source);

            source.Remove(4);
            source.RemoveAt(1);

            subject.ToArray().ShouldBe(new[] { 5, 2, 1 });
        }

        [Fact]
        public void Replaces_values_upon_notification()
        {
            var subject = new ObservableView<int>(source);

            source[2] = 10;

            subject.ToArray().ShouldBe(new[] { 5, 4, 10, 2, 1 });
        }

        [Fact]
        public void Handles_source_collection_reset()
        {
            var subject = new ObservableView<int>(source);

            source.Add(3);
            source.Add(5);
            source.Clear();
            source.Add(2);
            source.Add(4);

            subject.ToArray().ShouldBe(new[] { 2, 4 });
        }
    }
}