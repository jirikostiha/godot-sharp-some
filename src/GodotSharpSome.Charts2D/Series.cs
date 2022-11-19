namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;
    using Godot;

    public record Series<T> : Series
    {
        private Func<T, float>? _xValueGetter;

        private Func<T, float> _yValueGetter;

        public Series(Func<T, float> yValueGetter)
        {
            _yValueGetter = yValueGetter;
        }

        public Series(Func<T, float>? xValueGetter, Func<T, float> yValueGetter)
        {
            _xValueGetter = xValueGetter;
            _yValueGetter = yValueGetter;
        }

        public List<T> Objecs { get; set; } = new List<T>();

        public override int Count { get; }

        //public override (float, float) this[int index]
        //{
        //    get => 
        //        (_xValueGetter == null ? index : _xValueGetter(Objecs[index]), 
        //        _yValueGetter(Objecs[index]));
        //}

        public override IEnumerable<Vector2> ScaledPoints
        {
            get
            {
                for (int i = 0; i < Objecs.Count; i++)
                {
                    yield return new Vector2(_xValueGetter == null ? i : _xValueGetter(Objecs[i]),
                        _yValueGetter(Objecs[i]));
                }
            }
        }
    }

    public abstract record Series : SeriesInfo
    {
        public float PointRadius { get; set; } = 1;

        public LineOptions? PointConnection { get; set; }

        public abstract int Count { get; }

        //public abstract (float, float) this[int index] { get; }

        public abstract IEnumerable<Vector2> ScaledPoints { get; }
    }

    public abstract record SeriesInfo
    {
        public string? Name { get; set; }

        public Color? Color { get; set; }
    }
}
