namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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

        [Fact]
        public void Filter_respects_item_changes()
        {
            var obj = new MyObject { Value = 0 };
            var obj2 = new MyObject { Value = 2 };
            var obj3 = new MyObject { Value = 3 };
            var objects = new ObservableCollection<MyObject> { obj2, obj, obj3, obj };

            var subject = new ObservableView<MyObject>(objects, x => x.Value > 0);

            obj.ChangeValue(10);
            obj2.ChangeValue(-1);

            subject.ToArray().ShouldBe(new[] { obj, obj3, obj });
        }

        [Fact]
        public void Filter_ignores_removed_items()
        {
            var obj1 = new MyObject { Value = 1 };
            var obj2 = new MyObject { Value = 2 };
            var obj3 = new MyObject { Value = 3 };
            var objects = new ObservableCollection<MyObject> { obj1, obj2, obj3 };

            var subject = new ObservableView<MyObject>(objects, x => x.Value%2 == 1);

            objects.Remove(obj2);
            obj2.ChangeValue(5);

            subject.ToArray().ShouldBe(new[] { obj1, obj3 });
        }

        [Fact]
        public void Filter_ignores_removed_items_even_after_source_reset()
        {
            var obj1 = new MyObject { Value = 1 };
            var obj2 = new MyObject { Value = 2 };
            var obj3 = new MyObject { Value = 3 };
            var objects = new ObservableCollection<MyObject> { obj1, obj2, obj3 };

            var subject = new ObservableView<MyObject>(objects, x => x.Value % 2 == 1);

            objects.Clear();
            obj2.ChangeValue(5);

            subject.ShouldBeEmpty();
        }
    }

    public class MyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int Value { get; set; }

        public void ChangeValue(int value)
        {
            Value = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }
    }
}