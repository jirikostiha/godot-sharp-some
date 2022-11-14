using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class SegmenedLines : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.SegmentedLine(LeftMiddle(1), RightMiddle(1), segmentCount: 4),
            LineColor);

        DrawMultiline(
            Multiline.SegmentedArrow(LeftTop(2), RightBottom(3), segmentLength: 20, headRadius: 15),
            LineColor);
    }
}
