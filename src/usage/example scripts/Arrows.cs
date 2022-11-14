using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Arrows: ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Arrow(LeftBottom(1), RightTop(1), headRadius: 15),
            LineColor);

        DrawMultiline(
            Multiline.Arrow(LeftBottom(2), RightTop(2), headRadius: 10, arrowAngle: Pi * 5 / 6),
            LineColor);

        DrawMultiline(
            Multiline.DoubleArrow(LeftBottom(3), RightTop(3), headRadius: 15),
            LineColor);

        DrawDimmensionLength(4);
    }

    void DrawDimmensionLength(int column)
    {
        var a = LeftMiddle(column);
        var b = RightMiddle(column);
        var points = new List<Vector2>(14);
        Multiline.AppendLine(points, a + new Vector2(0, 8), a + new Vector2(0, -4));
        Multiline.AppendLine(points, b + new Vector2(0, 8), b + new Vector2(0, -4));
        Multiline.AppendDoubleArrow(points, a, b, 16);
        DrawMultiline(points.ToArray(), LineColor);
        DrawString(GetFont(null), Middle(column) + new Vector2(-8, -3), "42", TextColor);
    }
}
