using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Dots : ExampleNodeBase
{
    private float _time;

    private float[] _sinSamplePointsX = Enumerable.Range(0, 150).Select(i => 2f * i).ToArray();

    private int[] _powerSamplePointsX = Enumerable.Range(-25, 51).ToArray();
    private int _powerPointCount;

    public Dots()
    {
        _powerPointCount = _powerSamplePointsX.Length;
    }

    protected override void NextState(float delta)
    {
        _time += 0.02f;

        _powerPointCount = _powerPointCount <= _powerSamplePointsX.Length
            ? _powerPointCount + 1
            : 0;
    }

    public override void _Draw()
    {
        // I
        Drawlines(
            LeftBottom(1),
            RowHeight / 3,
            CellWidth / 2,
            CellWidth);

        // II
        DrawPower(MiddleBottom(2));

        // III
        DrawSin(LeftMiddle(3));
    }

    void Drawlines(Vector2 origin, float ystep, float minLength, float maxLength)
    {
        DrawMultiline(
            Multiline.DottedLine(
                origin,
                origin + (minLength + Abs(Cos(_time)) * (maxLength - minLength)) * Vector2.Right),
            LineColor);

        DrawMultiline(
            Multiline.DashedLine(
                origin + ystep * Vector2.Down,
                origin + ystep * Vector2.Down + (minLength + Abs(Cos(_time)) * (maxLength - minLength)) * Vector2.Right),
            LineColor);

        DrawMultiline(
            Multiline.DashDottedLine(
                origin + 2 * ystep * Vector2.Down,
                origin + 2 * ystep * Vector2.Down + (minLength + Abs(Cos(_time)) * (maxLength - minLength)) * Vector2.Right),
            LineColor);
    }

    void DrawPower(Vector2 origin)
    {
        var functionPoints = _powerSamplePointsX.Take(_powerPointCount).Select(x => origin +
            new Vector2(x, (x / 10f) * (x / 10f) * 10));

        var m = new Multiline()
            .AppendDottedLine(origin + new Vector2(0, -4), origin + new Vector2(0, 70), 8)
            .AppendDottedLine(origin + new Vector2(-30, 0), origin + new Vector2(30, 0), 8)
            .AppendDots(functionPoints);

        DrawMultiline(m.Points, LineColor);
    }

    void DrawSin(Vector2 start)
    {
        var points = _sinSamplePointsX.Select(x => start + new Vector2(x, 40 * Sin(_time + 0.1f * x)));
        this.DrawDots(points.ToArray(), LineColor);
    }
}
