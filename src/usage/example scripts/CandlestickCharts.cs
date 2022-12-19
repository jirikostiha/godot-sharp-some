using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class CandlestickCharts : ExampleNodeBase
{
    private Queue<(float Open, float Low, float High, float Close)> _frames = new();
    private int _countLimit = 40;
    private float _speedCounter = 0;
    private float _speedTreshold = 0.05f;
    private float _candleHalfWidth = 4f;
    private float _lastClose = 7;

    public CandlestickCharts()
    {
        for (int i = 0; i < _countLimit; i++)
        {
            var frame = NextFrame(_lastClose, 5, 20);
            _lastClose = frame.Close;
            _frames.Enqueue(frame);
        }
    }

    protected override void NextState(float delta)
    {
        _speedCounter += delta;
        if (_speedCounter < _speedTreshold)
            return;

        _speedCounter = 0;

        if (_frames.Count >= _countLimit)
            _frames.Dequeue();

        var frame = NextFrame(_lastClose, 0, 17);
        _lastClose = frame.Close;
        _frames.Enqueue(frame);
    }

    public override void _Draw()
    {
        DrawChart(LeftBottom(1));
    }

    private (float Open, float Low, float High, float Close) NextFrame(float open, float min, float max)
    {
        var low = Max(open - NextFloat(0, 2), min);
        var high = Min(open + NextFloat(0, 2), max);
        return (
            open,
            low,
            high,
            low + NextFloat(0, high - low));
    }

    private void DrawChart(Vector2 origin)
    {
        var points = new List<Vector2>();
        var xUnit = 12;
        Multiline.AppendAxes(points, origin, Vector2.Right, xUnit, 52, 10, 34);
        DrawMultiline(points.Select(FlipY).ToArray(), LineColor);

        DrawColoredCandles(origin, xUnit, 20, 20);
    }

    private void DrawColoredCandles(Vector2 origin, float xUnitStep, float yUnitStep, int yOffset)
    {
        int i = 1;
        foreach (var frame in _frames)
        {
            var lowPoint = origin + new Vector2(i * xUnitStep, yOffset + frame.Low * yUnitStep);
            var highPoint = origin + new Vector2(i * xUnitStep, yOffset + frame.High * yUnitStep);
            var bottomOffset = (Min(frame.Open, frame.Close) - frame.Low) * yUnitStep;
            var topOffset = (frame.High - Max(frame.Open, frame.Close)) * yUnitStep;

            this.DrawCandlestick(
                low: FlipY(lowPoint),
                lowOffset: bottomOffset,
                high: FlipY(highPoint),
                highOffset: topOffset,
                halfWidth: _candleHalfWidth,
                lineColor: LineColor,
                bodyColor: frame.Close > frame.Open ? Color.ColorN("green") : Color.ColorN("red"));

            i++;
        }
    }

    private Vector2 FlipY(Vector2 position) => new (position.x, RectSize.y - position.y);
}
