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
            Multiline.Candlestick(
                BottomMiddle(1), bottomOffset: 30,
                TopMiddle(1), topOffset: 16, 4f),
            LineColor);
    }
}
