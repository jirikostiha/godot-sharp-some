namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections.Generic;

    public abstract record Series<T> : SeriesInfo
    {
        protected Series(Func<T, float>? xValueGetter = null)
        {
            XValueGetter = xValueGetter;
        }

        protected Func<T, float>? XValueGetter { get; set; }

        public IList<T> Data { get; set; } = new List<T>();
    }
}
