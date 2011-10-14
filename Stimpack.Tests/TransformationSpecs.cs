namespace Stimpack
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Should;

    using Xunit;

    public class TransformationSpecs
    {
        readonly ObservableCollection<int> source;

        public TransformationSpecs()
        {
            source = new ObservableCollection<int>();
        }

        [Fact]
        public void Transformation_is_readonly()
        {
            IList<string> trans = source.Transform(x => x.ToString());

            trans.IsReadOnly.ShouldBeTrue();
            Assert.Throws<InvalidOperationException>(() => trans.Add(""));
            Assert.Throws<InvalidOperationException>(() => trans.Clear());
            Assert.Throws<InvalidOperationException>(() => trans.Remove(""));
            Assert.Throws<InvalidOperationException>(() => trans.RemoveAt(0));
            Assert.Throws<InvalidOperationException>(() => trans.Insert(0, ""));
            Assert.Throws<InvalidOperationException>(() => trans[0] = "");
        }

        [Fact]
        public void Support_for_non_notifying_collections()
        {
            new[] { 1, 2, 3, 4 }
                .Transform(x => x.ToString())
                .ToArray()
                .ShouldEqual(new[] { "1", "2", "3", "4" });
        }

        [Fact]
        public void Adds_new_values_upon_notification()
        {
            var trans = CreateToStringTransformation();

            source.Add(3);
            source.Add(5);
            source.Insert(0, 2);
            source.Insert(2, 4);

            trans.ToArray().ShouldEqual(new[] { "2", "3", "4", "5" });
        }

        [Fact]
        public void Removes_values_upon_notification()
        {
            source.Add(2);
            source.Add(3);
            source.Add(4);
            source.Add(5);
            var trans = CreateToStringTransformation();

            source.Remove(4);
            source.RemoveAt(1);

            trans.ToArray().ShouldEqual(new[] { "2", "5" });
        }

        [Fact]
        public void Replaces_values_upon_notification()
        {
            source.Add(2);
            source.Add(3);
            source.Add(4);
            source.Add(5);
            var trans = CreateToStringTransformation();

            source[2] = 10;

            trans.ToArray().ShouldEqual(new[] { "2", "3", "10", "5" });
        }

        [Fact]
        public void Handles_source_collection_reset()
        {
            var trans = CreateToStringTransformation();

            source.Add(3);
            source.Add(5);
            source.Clear();
            source.Add(2);
            source.Add(4);

            trans.ToArray().ShouldEqual(new[] { "2", "4" });
        }

        IEnumerable<string> CreateToStringTransformation()
        {
            var tr = source.Transform(x => x.ToString());
            return tr;
        }
    }
}