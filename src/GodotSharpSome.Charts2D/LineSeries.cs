namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections.Generic;
    using Godot;

    public interface ILineSeries
    {
        public IEnumerable<Vector2> Points { get; }
    }

    public record LineSeries<T> : Series<T>, ILineSeries
    {
        public LineSeries(Func<T, float> yValueGetter) 
            : this(null, yValueGetter)
        {}

        public LineSeries(Func<T, float>? xValueGetter, Func<T, float> yValueGetter)
            : base(xValueGetter)
        {
            YValueGetter = yValueGetter;
        }

        protected Func<T, float> YValueGetter { get; set; }

        public float PointRadius { get; set; } = 1;

        public LineOptions? ConnectionLine { get; set; }

        public Vector2 this[int index]
        {
            get => new (
                XValueGetter is null ? index : XValueGetter(Data[index]),
                YValueGetter(Data[index]));
        }

        public IEnumerable<Vector2> Points
        {
            get
            {
                for (int i = 0; i < Data.Count; i++)
                    yield return this[i];
            }
        }
    }

    public record LineSeries : Series<Vector2>, ILineSeries
    {
        public float PointRadius { get; set; } = 1;

        public LineOptions? ConnectionLine { get; set; }

        public Vector2 this[int index] => Data[index];

        public IEnumerable<Vector2> Points => Data;
    }
}
