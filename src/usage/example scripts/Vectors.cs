using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Vectors : ExampleNodeBase
{
    private static Color AxesColor = Color.ColorN("gray");
    private static Color VectorSumColor = Color.ColorN("magenta");

    private readonly List<Vector2> _vectors;

    public Vectors()
    {
        _vectors = new List<Vector2>
        {
            new Vector2(40, 40),
            new Vector2(170, 60),
            new Vector2(80, 120),
            new Vector2(40, -40),
            new Vector2(0, 130),
            new Vector2(-80, 20),
            new Vector2(-20, -50),
            new Vector2(-50, 5),
            new Vector2(-90, 40)
        };
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.VectorsAbsolutely(new Vector2(100, 100), _vectors),
            LineColor);

        var origin = new Vector2(300, 10);
        DrawMultiline(
            Multiline.Axes(origin, Vector2.Right, 50f, 7, 50f, 7),
            AxesColor);

        DrawMultiline(
            Multiline.VectorsRelatively(origin, _vectors),
            LineColor);

        DrawMultiline(
            Multiline.Arrow(origin, origin + _vectors.Aggregate((a, b) => a + b)),
            VectorSumColor);
    }
}
