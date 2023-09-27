namespace GodotSharpSome.Charts2D
{
    using Godot;
    using System;
    using System.Collections.Generic;

    public interface ICandleSeries
    {
        IEnumerable<(float X, Candle Candle)> Candles { get; }
    }

    public record CandleSeries<T> : Series<T>, ICandleSeries
    {
        private Func<T, float> _lowGetter;

        private Func<T, float> _openGetter;

        private Func<T, float> _closeGetter;

        private Func<T, float> _highGetter;

        public CandleSeries(Func<T, float> lowGetter, Func<T, float> highGetter, Func<T, float> openGetter, Func<T, float> closeGetter)
            : this(null, lowGetter, highGetter, openGetter, closeGetter)
        {}

        public CandleSeries(Func<T, float>? xValueGetter, Func<T, float> lowGetter, Func<T, float> highGetter, Func<T, float> openGetter, Func<T, float> closeGetter)
            : base(xValueGetter)
        {
            _lowGetter = lowGetter;
            _highGetter = highGetter;
            _openGetter = openGetter;
            _closeGetter = closeGetter;
        }

        /// <summary>
        /// Get scaled value
        /// </summary>
        public Candle this[int index]
        {
            get => new()
            {
                Low = _lowGetter(Data[index]),
                High = _highGetter(Data[index]),
                Open = _openGetter(Data[index]),
                Close = _closeGetter(Data[index])
            };
        }

        public IEnumerable<(float X, Candle Candle)> Candles
        {
            get
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    yield return (
                        XValueGetter is null ? i : XValueGetter(Data[i]),
                        new Candle()
                        {
                            Low = _lowGetter(Data[i]),
                            High = _highGetter(Data[i]),
                            Open = _openGetter(Data[i]),
                            Close = _closeGetter(Data[i])
                        });
                };
            }
        }
    }

    public record CandleSeries : Series<(float X, Candle Candle)>, ICandleSeries
    {
        /// <summary>
        /// Get scaled value
        /// </summary>
        public Candle this[int index] => Data[index].Candle;

        public IEnumerable<(float X, Candle Candle)> Candles
        {
            get
            {
                for (int i = 0; i < Data.Count; i++)
                    yield return Data[i];
            }
        }
    }
}
