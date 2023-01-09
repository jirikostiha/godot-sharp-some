using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Circles : ExampleNodeBase
{
    private float _time;

    protected override void NextState(float delta)
    {
        _time += delta;
    }

    public override void _Draw()
    {
        // I, II, III
        DrawSizing(Middle(1), CellWidth * Vector2.Right, 40, 3);

        // IV
        DrawKittLightEffect(LeftMiddle(4), 10, 30 * Vector2.Right, 8);
    }

    private void DrawSizing(Vector2 origin, Vector2 offset, float baseRadius, float radiusStep)
    {
        var count = Max(1, (int)(Sin(_time * 5) * (baseRadius / radiusStep)));

        for (int i = 0; i < count; i++)
        {
            var radius = baseRadius - i * radiusStep;
            
            // I
            this.DrawCircleLine(origin, radius, LineColor.Lightened(0.08f * i));
            
            // II
            this.DrawCircleArea(origin + offset, radius, AreaColor.Lightened(0.08f * i));
            
            // III
            this.DrawCircle(origin + 2 * offset, radius, LineColor.Lightened(0.08f * i), AreaColor.Lightened(0.08f * i));
        }
    }

    private void DrawKittLightEffect(Vector2 refPoint, float baseRadius, Vector2 distanceStep, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var aux = Cos(1 * _time + i * (Pi / count));
            this.DrawCircleLine(
                refPoint + distanceStep * i,
                aux >= 0 
                    ? 2 + baseRadius * aux 
                    : 2 + baseRadius * Cos(1 * _time + (Pi - i * (Pi / (count-1)))),
                //2 + baseRadius * Abs(Cos(1 * _time + i * (Pi / count))),
                LineColor);
        }
    }
}
