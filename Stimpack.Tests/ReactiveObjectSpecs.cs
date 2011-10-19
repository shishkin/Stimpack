namespace Stimpack
{
    using System.Reactive.Subjects;

    using Shouldly;

    using Xunit;

    public class ReactiveObjectSpecs
    {
        readonly static Subject<int> Source = new Subject<int>();
        readonly Dummy target = new Dummy();

        [Fact]
        public void Property_changes_upon_notifications()
        {
            Source.OnNext(20);

            target.MyProp.ShouldBe(10);
        }

        [Fact]
        public void Provides_component_model_notifications()
        {
            string property = null;
            target.PropertyChanged += (s, e) => { property = e.PropertyName; };

            Source.OnNext(10);

            property.ShouldBe("MyProp");
        }

        class Dummy : ReactiveObject
        {
            public Dummy()
            {
                Upon(Source, () => MyProp, x => x / 2);
            }

            public int MyProp { get; set; }
        }
    }
}