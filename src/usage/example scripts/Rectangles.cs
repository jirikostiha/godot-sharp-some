using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Rectangles : ExampleNodeBase
{
    public override void _Draw()
    {
        var length = 68f;
        var width = 24f;
        var rotationAngle = Pi / 5.1f;

        this.DrawRectangleLine(Middle(1), length, width, rotationAngle, LineColor);

        this.DrawRectangleArea(Middle(2), length, width, rotationAngle, AreaColor);

        this.DrawRectangle(Middle(3), length, width, rotationAngle, LineColor, AreaColor);
    }
}
