using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Crosses : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Cross(Middle(1), radius: 15),
            LineColor);

        DrawMultiline(
            Multiline.Cross2(Middle(2), outerRadius: 15, innerRadius: 5),
            LineColor, width: 2);
    }
}
