using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Dots : ExampleNodeBase
{
    private float[] _sinSamplePointsX = Enumerable.Range(0, 150).Select(i => 2f * i).ToArray();
    private float _sinOffset;

    private int[] _powerSamplePointsX = Enumerable.Range(-25, 51).ToArray();
    private int _powerPointCount;

    public Dots()
    {
        _powerPointCount = _powerSamplePointsX.Length;
    }

    protected override void NextState(float delta)
    {
        _sinOffset += 0.02f;

        _powerPointCount = _powerPointCount <= _powerSamplePointsX.Length
            ? _powerPointCount + 1
            : 0;
    }

    public override void _Draw()
    {
        // I
        DrawMultiline(
            Multiline.DottedLine(LeftBottom(1), RightBottom(1)),
            LineColor);
        DrawMultiline(
            Multiline.DashedLine(LeftMiddle(1), RightMiddle(1)),
            LineColor);
        DrawMultiline(
            Multiline.DashDottedLine(LeftTop(1), RightTop(1)),
            LineColor);

        // II
        DrawPower(MiddleBottom(2));

        // III
        DrawSin(LeftMiddle(3));
    }
    void DrawSin(Vector2 start)
    {
        var points = _sinSamplePointsX.Select(x => start + new Vector2(x, 40 * Sin(_sinOffset + 0.1f * x)));
        this.DrawDots(points.ToArray(), LineColor);
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
}
