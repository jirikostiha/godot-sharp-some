using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Candlesticks : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Candlestick(TopMiddle(1), 20, BottomMiddle(1), 10, 5),
            LineColor);

        DrawMultiline(
            Multiline.CrossedCandlestick(LeftTop(2), 20, LeftBottom(2), 10, 5, true),
            LineColor);
        DrawMultiline(
            Multiline.CrossedCandlestick(TopMiddle(2), 20, BottomMiddle(2), 10, 5, false),
            LineColor);

        this.DrawCandlestick(LeftTop(3), 20, LeftBottom(3), 10, 5, LineColor, Color.ColorN("green"));
    }
}
