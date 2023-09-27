using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Charts2D;
using static Godot.Mathf;

public class CandlestickChartExample : Chart
{
    private int _iteration = 0;

    private CandleSeries _series = new CandleSeries();

    public override void _Ready()
    {
        _series = new CandleSeries()
        {
            Data = TestCandles().Select((c,i)=> ((float)i, c)).ToList(),
            Color = Color.ColorN("red")
        };

        Series.Add(_series);

        var s2 = new CandleSeries<Candle>(f => f.Low, f => f.High, f => f.Open, f => f.Close)
        {
            Data = GenerateFrames(3, 0.2f, 0, 1).ToList(),
            Color = Color.ColorN("gray")
        };

        //Series.Add(s2);

        XMax = 20;
    }

    public override void _PhysicsProcess(float delta)
    {
        _iteration++;
        if (_iteration % 10 == 0)
        {
            _series.Data.Add((default,
                new Candle() { Open = _series.Data[_series.Data.Count -1].Candle.Close }));
        }
        else
        {
            var newValue = _series.Data[_series.Data.Count - 1].Candle.Close + NextFloat(-0.1f, 0.1f) + 0.05f * Sin(_iteration/20);
            UpdateFrame(_series.Data[_series.Data.Count -1].Candle, newValue);
        }
    }

    private IEnumerable<Candle> GenerateFrames(int count, float open, float min, float max)
    {
        for (int i = 0; i < count; i++)
            yield return NextFrame(open, min, max);
    }

    private IEnumerable<Candle> TestCandles()
    {
        yield return new() { Low = 0, High = 1, Open = 0, Close = 1 };
        yield return new() { Low = 0, High = 1, Open = 0.5f, Close = 0.5f };
        yield return new() { Low = 0, High = 1, Open = 0.2f, Close = 0.8f };
        yield return new() { Low = 0, High = 1, Open = 0.8f, Close = 0.2f };
        yield return new() { Low = 0.5f, High = 1, Open = 0.6f, Close = 0.9f };
        yield return new() { Low = 0.5f, High = 1, Open = 0.9f, Close = 0.6f };
        yield return new() { Low = 0, High = 0.5f, Open = 0.1f, Close = 0.4f };
        yield return new() { Low = 0, High = 0.5f, Open = 0.4f, Close = 0.1f };
    }

    private Candle NextFrame(float open, float min, float max)
    {
        var low = Max(open - NextFloat(0, 0.3f * (max - min)), min);
        var high = Min(open + NextFloat(0, 0.3f * (max - min)), max);
        return new Candle
        {
            Open = open,
            Low = low,
            High = high,
            Close = low + NextFloat(0, high - low)
        };
    }

    private float NextFloat(float inclusiveMin, float exclusiveMax) 
        => ExampleNodeBase.NextFloat(inclusiveMin, exclusiveMax);

    private void UpdateFrame(Candle frame, float value)
    {
        if (value < frame.Low) 
            frame.Low = value;
        else if (value > frame.High)
            frame.High = value;

        frame.Close = value;
    }
}

