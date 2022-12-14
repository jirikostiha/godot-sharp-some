using Godot;
using GodotSharpSome.Drawing2D;

public class Polygons : ExampleNodeBase
{
    private float _angle = 0;

    protected override void NextState(float delta)
    {
        _angle += 0.1f;
    }

    public override void _Draw()
    {
        for (int i = 4; i >= 0; i--)
        {
            var r = 5 + i * 10;
            var n = i + 3;
            var angle = i % 2 == 0 ? _angle : -_angle;
            var areaColor = AreaColor.Darkened(i * 0.1f);
            this.DrawRegularConvexPolygonLine(Middle(1), r, n, angle, LineColor);
            this.DrawRegularConvexPolygonArea(Middle(2), r, n, angle, areaColor);
            this.DrawRegularConvexPolygon(Middle(3), r, n, angle, LineColor, areaColor);
        }
    }

    public void _on_Animate_pressed() => Animate = !Animate;
}
