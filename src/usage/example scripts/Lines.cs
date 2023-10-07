using Godot;
using GodotSharpSome.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using static Godot.Mathf;

namespace GodotSharpSome.Examples;

public partial class Lines : ExampleNodeBase
{
    private float _value;

    private Tween _tween;

    public override void _Ready()
    {
        StartTween();
    }

    public override void _Draw()
    {
        // I
        DrawLineTypes(LeftBottom(1), RowHeight / 3f, CellWidth / 2f, CellWidth);

        // II
        DrawContinuationLineByDiffTypes(LeftBottom(2));

        // III
        DrawLineFromRef(LeftBottom(3));

        // IV
        DrawParallelLine(LeftBottom(4));

        // V
        DrawParallelLines(LeftBottom(5), RightTop(5));
    }

    private void DrawLineTypes(Vector2 origin, float ystep, float minLength, float maxLength)
    {
        DrawMultiline(
            DottedLine.AppendLine(
                new List<Vector2>(30),
                origin,
                origin + (minLength + _value * (maxLength - minLength)) * Vector2.Right)
            .ToArray(),
            LineColor);

        DrawMultiline(
            DashedLine.AppendLine(
                new List<Vector2>(),
                origin + ystep * Vector2.Down,
                origin + ystep * Vector2.Down + (minLength + _value * (maxLength - minLength)) * Vector2.Right)
            .ToArray(),
            LineColor);

        DrawMultiline(
            DashDottedLine.AppendLine(
                new List<Vector2>(30),
                origin + 2 * ystep * Vector2.Down,
                origin + 2 * ystep * Vector2.Down + (minLength + _value * (maxLength - minLength)) * Vector2.Right)
            .ToArray(),
            LineColor);
    }

    private Multiline _ml = new(
        (nameof(LineType.Solid), new SolidLine()),
        (nameof(LineType.Dotted), new DottedLine()),
        (nameof(LineType.Dashed), new DashedLine()));

    private DashDottedLine _ddLine = new(dashLength: 10, spaceLength: 4);

    private void DrawContinuationLineByDiffTypes(Vector2 origin)
    {
        var start = origin + 10 * Vector2.Right;

        var points = _ml
            .Clear()
            .SetPen("Solid")
            .AppendLine(start, start + Vector2.Right * 60)
            .SetPen(nameof(LineType.Dotted))
            .AppendLine(start + Vector2.Right * 60 + Vector2.Down * 60)
            .SetPen(2)
            .AppendLine(start + Vector2.Down * 60)
            .SetPen(_ddLine)
            .AppendLine(start)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawLineFromRef(Vector2 origin)
    {
        var start = origin + Vector2.Right * 25;

        // create line directly by appender's static method
        DrawMultiline(SolidLine.Line(origin, start), new Color("lightblue"));

        // multi line as local variable -> always allocating memory -> not recommended
        var ml = new Multiline(8 * 2);
        for (int i = 0; i < 8; i++)
            ml.AppendLineFromRef(origin, start, i * Pi / 2f / 7, 40);

        DrawMultiline(ml.Points(), LineColor);

        ml.Clear().AppendLine(origin + Vector2.Down * 50, origin + Vector2.Down * 50 + Vector2.Right * 25);
        for (int i = 0; i < 8; i++)
            ml.AppendLineFromRef((1 + i) * Pi / 12f, 15 - i);

        DrawMultiline(ml.Points(), LineColor);
    }

    private void DrawParallelLine(Vector2 start)
    {
        var end = start + new Vector2(70, 20);

        DrawMultiline(
            SolidLine.Line(start, end),
            LineColor.Lightened(0.3f));

        DrawMultiline(
            new Multiline().AppendParallelLine(start, end, 10).Points(),
            LineColor);

        DrawMultiline(
            new Multiline().AppendParallelLine(start, end, 20, 10, -10).Points(),
            LineColor);

        DrawMultiline(
            new Multiline().AppendParallelLine(start, end, 30, -10, 10).Points(),
            LineColor);
    }

    private void DrawParallelLines(Vector2 corner1, Vector2 corner2)
    {
        DrawMultiline(
            new Multiline()
            .AppendParallelLines(
                corner1,
                corner1 + new Vector2(corner2.X - corner1.X, 0),
                (corner2.Y - corner1.Y) / 7f,
                8)
            .Points(),
            LineColor);

        DrawMultiline(
            new Multiline()
            .AppendParallelLines(
                corner1 + new Vector2(0, corner2.Y - corner1.Y),
                corner1,
                new float[] { 0, 10, 20, 30, 20 })
            .Points(),
            LineColor);
    }

    public void Interpolate(float value)
    {
        _value = value;
    }

    private void StartTween()
    {
        _tween = CreateTween().SetLoops();
        _tween.TweenMethod(new Callable(this, nameof(Interpolate)), 0f, 1f, 2f);
        _tween.TweenMethod(new Callable(this, nameof(Interpolate)), 1f, 0f, 2f);
    }
}