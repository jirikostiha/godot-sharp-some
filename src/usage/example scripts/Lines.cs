using System;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Lines : ExampleNodeBase
{
    private float[] Bounds = new float[] { 0f, 1f };

    private float _value;

    private const float Step = 0.5f;

    public Tween Tween { get; set; }

    public override void _Ready()
    {
        Tween = GetNode<Tween>("Tween");
        StartTween();
    }

    public override void _Draw()
    {
        // I
        DrawLineTypes(LeftBottom(1), RowHeight / 3, CellWidth / 2, CellWidth);

        // II
        DrawContinuationLine(LeftBottom(2));
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
            .Points;
            
        DrawMultiline(points, LineColor);
    }

    public void Interpolate(float value)
    {
        _value = value;
    }

    public void _on_Tween_completed(object obj, NodePath path)
    {
        Array.Reverse(Bounds);
        StartTween();
    }
    private void StartTween()
    {
        Tween.InterpolateMethod(this, nameof(Interpolate), Bounds[0], Bounds[1], 2f);
        Tween.Start();
    }
}
