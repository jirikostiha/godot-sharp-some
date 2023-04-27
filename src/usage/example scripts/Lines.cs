using System;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public partial class Lines : ExampleNodeBase
{
    private float[] _bounds = new float[] { 0f, 1f };

    private float _value;

    private const float Step = 0.5f;

    public Tween Tween { get; set; }

    public override void _Ready()
    {
        StartTween();
    }

    public override void _Draw()
    {
        // I
        DrawLineTypes(LeftBottom(1), RowHeight / 3f, CellWidth / 2f, CellWidth);

        // II
        DrawContinuationLine(LeftBottom(2));

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
            Multiline.DottedLine(
                origin,
                origin + (minLength + _value * (maxLength - minLength)) * Vector2.Right),
            LineColor);

        DrawMultiline(
            Multiline.DashedLine(
                origin + ystep * Vector2.Down,
                origin + ystep * Vector2.Down + (minLength + _value * (maxLength - minLength)) * Vector2.Right),
            LineColor);

        DrawMultiline(
            Multiline.DashDottedLine(
                origin + 2 * ystep * Vector2.Down,
                origin + 2 * ystep * Vector2.Down + (minLength + _value * (maxLength - minLength)) * Vector2.Right),
            LineColor);
    }

    private void DrawContinuationLine(Vector2 start)
    {
        var points = new Multiline(4 * 2)
            .AppendLine(start, start + Vector2.Right * 60)
            .AppendLine(Vector2.Down * 50)
            .AppendLine(new Vector2(-40, -10))
            .AppendLine(new Vector2(30, -20))
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawLineFromRef(Vector2 start)
    {
        var refPoint = start + Vector2.Right * 25;
        DrawMultiline(Multiline.Line(start, refPoint), new Color("lightblue"));

        var ml = new Multiline(8 * 2);
        for (int i = 0; i < 8; i++)
            ml.AppendLineFromRef(start, refPoint, i * Pi/2f/7, 40);

        DrawMultiline(ml.Points(), LineColor);

        ml.Clear();
        ml.AppendLine(start + Vector2.Down * 50, start + Vector2.Down * 50 + Vector2.Right * 25);
        for (int i = 0; i < 8; i++)
            ml.AppendLineFromRef((1 + i) * Pi / 12f, 15 - i);

        DrawMultiline(ml.Points(), LineColor);
    }

    private void DrawParallelLine(Vector2 start)
    {
        var end = start + new Vector2(70, 20);
        DrawMultiline(
            Multiline.Line(start, end),
            LineColor.Lightened(0.3f));

        DrawMultiline(
            Multiline.ParallelLine(start, end, 10),
            LineColor);

        DrawMultiline(
            Multiline.ParallelLine(start, end, 20, 10, -10),
            LineColor);

        DrawMultiline(
            Multiline.ParallelLine(start, end, 30, -10, 10),
            LineColor);
    }

    private void DrawParallelLines(Vector2 corner1, Vector2 corner2)
    {
        DrawMultiline(
            Multiline.ParallelLines(
                corner1,
                corner1 + new Vector2(corner2.X - corner1.X, 0),
                (corner2.Y - corner1.Y) / 7f,
                8),
            LineColor);

        DrawMultiline(
            Multiline.ParallelLines(
                corner1 + new Vector2(0, corner2.Y - corner1.Y),
                corner1,
                new float[] { 0, 10, 20, 30, 20 }),
            LineColor);
    }

    public void Interpolate(float value)
    {
        _value = value;
    }

    public void _on_Tween_completed(object obj, NodePath path)
    {
        Array.Reverse(_bounds);
        StartTween();
    }

    private void StartTween()
    {
        Tween = CreateTween();
        Tween.TweenMethod(new Callable(this, nameof(Interpolate)), _bounds[0], _bounds[1], 2f);
    }
}
