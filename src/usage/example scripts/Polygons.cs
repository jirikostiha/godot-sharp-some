using GodotSharpSome.Drawing2D;

public class Polygons : ExampleNodeBase
{
    public override void _Draw()
    {
        this.DrawRegularConvexPolygonLine(Middle(1), 30, 5, 0.2f, LineColor);

        this.DrawRegularConvexPolygonArea(Middle(2), 30, 5, 0.2f, AreaColor);

        this.DrawRegularConvexPolygon(Middle(3), 30, 5, 0.2f, LineColor, AreaColor);
    }
}
