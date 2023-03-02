using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public partial class Circles : ExampleNodeBase
{
    private double _time;

    protected override void NextState(double delta)
    {
        _time += delta;
    }

    public override void _Draw()
    {
        // I, II, III
        DrawSizing(Middle(1), CellWidth * Vector2.Right, 40, 3);

        // IV
        DrawNightRiderEffect(LeftMiddle(4), 12, 3 * CellWidth - Margin - 12, 10);
    }

    private void DrawSizing(Vector2 origin, Vector2 offset, float baseRadius, float radiusStep)
    {
        var count = Max(1, (int)(Sin(_time * 5) * (baseRadius / radiusStep)));

        for (int i = 0; i < count; i++)
        {
            var radius = baseRadius - i * radiusStep;

            // I
            this.DrawCircleOutline(origin, radius, LineColor.Lightened(0.08f * i));

            // II
            this.DrawCircleRegion(origin + offset, radius, AreaColor.Lightened(0.08f * i));

            // III
            this.DrawCircle(origin + 2 * offset, radius, LineColor.Lightened(0.08f * i), AreaColor.Lightened(0.08f * i));
        }
    }

    private void DrawNightRiderEffect(Vector2 refPoint, float baseRadius, float distance, int count)
    {
        var cmin = refPoint + baseRadius * Vector2.Right;
        var relDist = distance * (1 + Sin(_time * 4)) / 2f;

        var step = distance / (count - 1);
        for (int i = 0; i < count; i++)
        {
            var dist = step * i;
            var c = cmin + dist * Vector2.Right;
            var refDist = Abs(relDist - dist);
            var r = Max(3, baseRadius * (1 - refDist / (distance * 0.7f)));
            this.DrawCircleOutline(c, (float)r, LineColor);
        }
    }
}
