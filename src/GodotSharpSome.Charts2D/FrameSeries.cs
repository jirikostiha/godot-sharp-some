namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections.Generic;

    public record FrameSeries<T> : SeriesInfo
    {
        private Func<T, float> _lowGetter;

        private Func<T, float> _openGetter;

        private Func<T, float> _closeGetter;

        private Func<T, float> _highGetter;

        public FrameSeries(Func<T, float> lowGetter, Func<T, float> highGetter, Func<T, float> openGetter, Func<T, float> closeGetter)
        {
            _lowGetter = lowGetter;
            _highGetter = highGetter;
            _openGetter = openGetter;
            _closeGetter = closeGetter;
        }

        public List<T> Values { get; set; } = new List<T>();

        public (float Low, float High, float Open, float Close) this[int index]
        {
            get => (
                _lowGetter(Values[index]),
                _highGetter(Values[index]),
                _openGetter(Values[index]),
                _closeGetter(Values[index]));
        }
    }
}
