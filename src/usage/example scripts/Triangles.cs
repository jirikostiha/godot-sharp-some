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
            MiddleBottom(1),
            LineColor);

        this.DrawTriangleArea(LeftTop(2),
            RightTop(2),
            MiddleBottom(2),
            AreaColor);

        this.DrawTriangle(LeftTop(3),
            RightTop(3),
            MiddleBottom(3),
            LineColor, AreaColor);
    }
}
