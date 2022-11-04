using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Vectors : Godot.ColorRect
{
    private static Color BackColor = Color.ColorN("white");
    private static Color TextColor = Color.ColorN("black");
    private static Color AxesColor = Color.ColorN("gray");
    private static Color VectorColor = Color.ColorN("black");
    private static Color VectorSumColor = Color.ColorN("magenta");

    private List<Vector2> _vectors;

    public Vectors()
    {
        _vectors = new List<Vector2>();
        _vectors.Add(new Vector2(40, 40));
        _vectors.Add(new Vector2(170, 60));
        _vectors.Add(new Vector2(80, 120));
        _vectors.Add(new Vector2(40, -40));
        _vectors.Add(new Vector2(0, 130));
        _vectors.Add(new Vector2(-80, 20));
        _vectors.Add(new Vector2(-20, -50));
        _vectors.Add(new Vector2(-50, 5));
        _vectors.Add(new Vector2(-90, 40));
    }

    public override void _Ready()
    {
        Color = BackColor;
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.VectorsAbsolutely(new Vector2(100, 100), _vectors),
            VectorColor);

        var origin = new Vector2(300, 10);
        DrawMultiline(
            Multiline.Axes(origin, Vector2.Right, 50f, 7, 50f, 7),
            AxesColor);

        DrawMultiline(
            Multiline.VectorsRelatively(origin, _vectors),
            VectorColor);

        DrawMultiline(
            Multiline.Arrow(origin, origin + _vectors.Aggregate((a, b) => a + b)),
            VectorSumColor);
    }
}
