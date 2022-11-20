using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Godot.Mathf;
using GodotSharpSome.Drawing2D;

public class Dots : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.DottedLine(LeftBottom(1), RightBottom(1)),
            LineColor);
        DrawMultiline(
            Multiline.DashedLine(LeftMiddle(1), RightMiddle(1)),
            LineColor);
        DrawMultiline(
            Multiline.DashDottedLine(LeftTop(1), RightTop(1)),
            LineColor);

        DrawPower(MiddleBottom(2));

        DrawSin(LeftMiddle(3));
    }
    void DrawSin(Vector2 start)
    {
        var points = Enumerable.Range(0, 160).Select(i => start + new Vector2(2 * i, 40 * Sin(0.1f * i)));
        DrawMultiline(Multiline.Dots(points), LineColor);
    }

    void DrawPower(Vector2 origin)
    {
        var functionPoints = Enumerable.Range(-25, 51)
            .Select(i => origin + new Vector2(i, (i / 10f) * (i / 10f) * 10));

        var m = new Multiline()
            .AppendDottedLine(origin + new Vector2(0, -4), origin + new Vector2(0, 70), 8)
            .AppendDottedLine(origin + new Vector2(-30, 0), origin + new Vector2(30, 0), 8)
            .AppendDots(functionPoints);

        DrawMultiline(m.Points, LineColor);
    }
}
