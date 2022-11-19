using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Arrows : ExampleNodeBase
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

        var v1 = LeftBottom(4);
        var v2 = BottomMiddle(5);
        var h = TopMiddle(5) - Middle(5);
        this.DrawRectangleLine(v1, v2, h.Length(), LineColor);
        this.DrawInnerDimmensionLength(v1, v2, Color.ColorN("darkgray"), GetFont(null), Color.ColorN("darkgray"));
    }
}
