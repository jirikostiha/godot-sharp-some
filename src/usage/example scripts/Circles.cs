using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Circles : ExampleNodeBase
{
    public override void _Draw()
    {
        var radius = 20;

        this.DrawCircleLine(Middle(1), radius, LineColor);

        this.DrawCircleArea(Middle(2), radius, AreaColor);

        this.DrawCircle(Middle(3), radius, LineColor, AreaColor);
    }
}
