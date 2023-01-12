using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Ellipses : ExampleNodeBase
{
    private float _time;
    private float _radius = 40;

    protected override void NextState(float delta)
    {
        _time += delta;
    }

    public override void _Draw()
    {
        // I
        this.DrawEllipseOutline(Middle(1), _radius, _radius * Sin(_time + 0.5f), _time * 3.3f, LineColor);

        // II
        this.DrawEllipseRegion(Middle(2), _radius, _radius * Sin(_time + 0.5f), _time * 3.3f, AreaColor);

        // III
        this.DrawEllipse(Middle(3), _radius, _radius * Sin(_time + 0.5f), _time * 3.3f, LineColor, AreaColor);
    }
}
