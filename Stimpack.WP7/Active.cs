namespace Stimpack
{
    public class Active<T> : Reactive<T>
    {
        public Active(T initialValue = default(T)) : base(initialValue) { }

        public new T Value
        {
            get { return base.Value; }
            set { OnNext(value); }
        }
    }
}