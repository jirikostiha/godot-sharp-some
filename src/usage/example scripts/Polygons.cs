using GodotSharpSome.Drawing2D;

namespace GodotSharpSome.Examples;

public partial class Polygons : ExampleNodeBase
{
    private double _time = 0;

    protected override void NextState(double delta) => _time += 0.1f;

    public override void _Draw()
    {
        for (int i = 4; i >= 0; i--)
        {
            var r = 5 + i * 10;
            var n = i + 3;
            var angle = i % 2 == 0 ? (float)_time : -(float)_time;
            var areaColor = AreaColor.Darkened(i * 0.1f);

            // I
            this.DrawRegularConvexPolygonOutline(Middle(1), r, n, angle, LineColor);

            // II
            this.DrawRegularConvexPolygonRegion(Middle(2), r, n, angle, areaColor);

            // III
            this.DrawRegularConvexPolygon(Middle(3), r, n, angle, LineColor, areaColor);
        }
    }
}