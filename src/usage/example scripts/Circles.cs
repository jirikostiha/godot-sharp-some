using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Circles : ExampleNodeBase
{
    private float _radiusStep = 3;
    private float _baseRadius = 40;
    private float _value;

    protected override void NextState(float delta)
    {
        _value += delta * 5;
    }

    public override void _Draw()
    {
        var count = Max(1, (int)(Sin(_value) * (_baseRadius / _radiusStep)));

        for (int i = 0; i < count; i++)
        {
            var radius = _baseRadius - i * _radiusStep;
            this.DrawCircleLine(Middle(1), radius, LineColor.Lightened(0.08f * i));
            this.DrawCircleArea(Middle(2), radius, AreaColor.Lightened(0.08f * i));
            this.DrawCircle(Middle(3), radius, LineColor.Lightened(0.08f * i), AreaColor.Lightened(0.08f * i));
        }
    }
}
