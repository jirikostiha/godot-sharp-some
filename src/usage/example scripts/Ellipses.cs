using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public partial class Ellipses : ExampleNodeBase
{
    private double _time;
    private float _radius = 40;

    protected override void NextState(double delta)
    {
        _time += delta;
    }

    public override void _Draw()
    {
        // I
        this.DrawEllipseOutline(Middle(1), _radius, _radius * Sin((float)_time + 0.5f), (float)_time * 3.3f, LineColor);

        // II
        this.DrawEllipseRegion(Middle(2), _radius, _radius * Sin((float)_time + 0.5f), (float)_time * 3.3f, AreaColor);

        // III
        this.DrawEllipse(Middle(3), _radius, _radius * Sin((float)_time + 0.5f), (float)_time * 3.3f, LineColor, AreaColor);
    }
}