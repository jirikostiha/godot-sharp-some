using Godot;
using static Godot.Mathf;

namespace GodotSharpSome.Drawing2D.Examples;

public partial class Mirroring : ExampleNodeBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Tween _tween;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private float _stateValue;

    private Multiline _ml = Multiline.FourLineTypes();

    public override void _Ready()
    {
        _tween = CreateTween().SetLoops();
        _tween.TweenMethod(new Callable(this, nameof(Interpolate)), 0f, 1f, 4f);
    }

    public override void _Draw()
    {
        // I
        DrawMirroredToY(Middle(1), CellWidth);

        // II
        DrawMirroredToX(Middle(2), CellWidth);

        // III
        DrawMirroredToLineByDirection(Middle(3), CellWidth);

        // IV
        DrawRotated(Middle(4), CellWidth);

        // V
        DrawRotated5(Middle(5), CellWidth);
    }

    private void DrawMirroredToY(Vector2 center, float size)
    {
        var points = _ml
            .Clear()
            .SetPen(0)
            .AppendTriangle(
                center + Vector2.Left * size * 0.35f + Vector2.Up * size * 0.4f,
                center + Vector2.Up * size * 0.1f,
                center + Vector2.Right * size * 0.35f + Vector2.Up * size * 0.4f)
            .MirrorX(center.X)
            .SetPen(3)
            .AppendLine(center + Vector2.Left * size / 2, center + Vector2.Right * size / 2)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawMirroredToX(Vector2 center, float size)
    {
        var points = _ml
            .Clear()
            .SetPen(0)
            .AppendTriangle(
                center + Vector2.Left * size * 0.05f,
                center + Vector2.Left * size * 0.05f + Vector2.Down * size * 0.4f,
                center + Vector2.Left * size * 0.4f + Vector2.Up * size * 0.4f)
            .MirrorY(center.X)
            .SetPen(3)
            .AppendLine(center + Vector2.Up * size / 2, center + Vector2.Down * size / 2)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawMirroredToLineByDirection(Vector2 center, float size)
    {
        var mp1 = center + Vector2.Right.Rotated(_stateValue * 2 * Pi) * size * 0.4f;
        var mp2 = center - Vector2.Right.Rotated(_stateValue * 2 * Pi) * size * 0.4f;

        var points = _ml
            .Clear()
            .SetPen(0)
            .AppendTriangle(
                center + Vector2.Left * size * 0.05f,
                center + Vector2.Left * size * 0.3f + Vector2.Down * size * 0.25f,
                center + Vector2.Left * size * 0.3f + Vector2.Up * size * 0.25f)
            .MirrorByPoints(mp1, mp2)
            .SetPen(3)
            .AppendLine(mp1, mp2)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawRotated(Vector2 center, float size)
    {
        this.DrawCircleRegion(center, 1, Colors.Red);

        var points = _ml
            .Clear()
            .SetPen(0)
            .AppendTriangle(
                center + Vector2.Left * size * 0.05f,
                center + Vector2.Left * size * 0.4f + Vector2.Down * size * 0.2f,
                center + Vector2.Left * size * 0.4f + Vector2.Up * size * 0.2f)
            .Rotate(center, Pi / 2)
            .Rotate(center, Pi)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void DrawRotated5(Vector2 center, float size)
    {
        this.DrawCircleRegion(center, 1, Colors.Red);

        var points = _ml
            .Clear()
            .SetPen(0)
            .AppendTriangle(
                center + Vector2.Left * size * 0.05f,
                center + Vector2.Left * size * 0.4f + Vector2.Down * size * 0.2f,
                center + Vector2.Left * size * 0.4f + Vector2.Up * size * 0.2f)
            .Rotate(center, 2 * Pi / 5f, 4)
            .Points();

        DrawMultiline(points, LineColor);
    }

    private void Interpolate(float value) => _stateValue = value;
}