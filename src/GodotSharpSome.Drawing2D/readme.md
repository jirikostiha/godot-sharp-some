# Godot Sharp Some - Drawing 2D

Is set of general extensions for custom drawing API in Godot engine.  

## CanvasItemExtensions

The CanvasItemExtensions type extends CanvasItem type by drawing methods of various objects from various parameters.
Generally there are three types of methods.

**Outline** postfix determines only outline (or perimeter) of the shape is drawn.

```cs
Draw<<Shape>>Outline(..)
```

\
**Region** postfix determines only plane region (or area) is drawn.

```cs
Draw<<Shape>>Region(..)
```

\
Without postfix the outline and region is drawn.

```cs
Draw<<Shape>>(..)
```

\
_Example C1_ shows drawing of solid geometric object.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleC1 : ColorRect
{
    public override void _Draw()
    {
        this.DrawCircleRegion(new Vector2(50, 50), 20, Colors.Blue)
            .DrawEllipse(new Vector2(50, 50), 30, 15, Pi / 2, Colors.Black, Colors.Green)
            .DrawMultiline(new Multiline().AppendCross(new Vector2(50, 50), 8).Points(), Colors.Gray);
    }
}
```

### Multiline

The multiline class is responsible for calculating coordinates of the points.  

_Example M1_ shows how to draw multiple vectors from common origin and then draw sum of vectors.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleM1 : ColorRect
{
    private _vectors = new Vector2[] { new(40, 40), new(70, 60), new(80, 120), new(40, -40), new(0, 30) };

    public override void _Draw()
    {
       DrawMultiline(
            new Multiline().AppendVectorsAbsolutely(new Vector2(100, 100), vectors).Points(),
            Colors.Gray);

        DrawMultiline(
            new Multiline().AppendVectorsRelatively(new Vector2(300, 300), vectors).Points(),
            Colors.Blue);
    }
}
```

_Example M2_ shows how to draw multiple types of line as one multiline.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleM2 : ColorRect
{
    private Multiline _ml = new(("solid", new SolidLine()), ("dotted", new DottedLine()), ("dashed", new DashedLine()));
    private DashDottedLine _ddLine = new(dashLength: 10, spaceLength: 4);

    public override void _Draw()
    {
        var start = Vector2.Zero;

        var points = _ml
            .Clear()
            .SetPen("solid")
            .AppendLine(start, start + Vector2.Right * 60)
            .SetPen("dotted")
            .AppendLine(start + Vector2.Right * 60 + Vector2.Down * 60)
            .SetPen(2)
            .AppendLine(start + Vector2.Down * 60)
            .SetPen(_ddLine)
            .AppendLine(start);
            .Points();

        DrawMultiline(points, Colors.Blue);
    }
}
```
