using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Triangles : ExampleNodeBase
{
    public override void _Draw()
    {
        this.DrawTriangleLine(LeftTop(1),
            RightTop(1),
            BottomMiddle(1),
            LineColor);

        this.DrawTriangleArea(LeftTop(2),
            RightTop(2),
            BottomMiddle(2),
            AreaColor);

        this.DrawTriangle(LeftTop(3),
            RightTop(3),
            BottomMiddle(3),
            LineColor, AreaColor);
    }
}
