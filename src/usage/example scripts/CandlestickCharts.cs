using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;

public class CandlestickCharts : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawChart(LeftBottom(1));
    }

    public void DrawChart(Vector2 origin)
    {
        var points = new List<Vector2>();
        var xUnit = 12;
        Multiline.AppendAxes(points, origin, Vector2.Right, xUnit, 52, 10, 34);
        AppendCandles(points, origin, xUnit, 20);

        DrawMultiline(points.ToArray(), LineColor);
    }

    public void AppendCandles(IList<Vector2> points, Vector2 origin, float xUnitStep, float yUnitStep)
    {
        var rand = new Godot.RandomNumberGenerator() { Seed = 2 };
        var bodyHalfWidth = 4f;

        var lastClose = 5f;
        for (int i = 1; i <= 50; i++)
        {
            var open = lastClose;
            var low = open - rand.Randf() * 2;
            var high = open + rand.Randf() * 3;
            var close = low + rand.Randf() * (high - low);

            var lowPoint = origin + new Vector2(i * xUnitStep, low * yUnitStep);
            var highPoint = origin + new Vector2(i * xUnitStep, high * yUnitStep);
            var bottomOffset = (Mathf.Min(open, close) - low) * yUnitStep;
            var topOffset = (high - Mathf.Max(open, close)) * yUnitStep;

            Multiline.AppendCrossedCandlestick(points,
                low: lowPoint,
                lowOffset: bottomOffset,
                high: highPoint,
                highOffset: topOffset,
                halfWidth: bodyHalfWidth,
                upDirrection: close > open);

            lastClose = close;
        }
    }
}
