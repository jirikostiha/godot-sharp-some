# Godot Sharp Some - Drawing 2D

Is set of general extensions for custom drawing API in Godot engine version 3.3 and higher.  

## CanvasItemExtensions

The CanvasItemExtensions type extends CanvasItem type by drawing methods of various objects from various parameters.
Generally there are three types of methods.

**Outline** postfix determines only outline (or perimeter) of the shape is drawn.

```cs
Draw<Shape>Outline(..)
```

\
**Region** postfix determines only plane region (or area) is drawn.

```cs
Draw<Shape>Region(..)
```

\
Without postfix the the outline and region is drawn.

```cs
Draw<Shape>(..)
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
        this.DrawCircleRegion(new Vector2(10, 10), new Vector2(100, 10), new Vector2(10, 100), Color.ColorN("black"))
            .DrawEllipse()
            .DrawMultiline(Multiline.Cross());
    }
}
```

### Multiline

The multiline class is responsible for calculating coordinates of the points and memory allocations.
There are three alternatives how to draw line-based figures via the multiline.  

**1) Quick**  
Just call predefined compositions.  

```cs
DrawMultiline(Multiline.<CompositionName>(..), <Color>);
```

pros: easy to use  
cons: bigger memory footprint

_Example M1_ shows how to draw vectors from common origin and then draw sum of vectors.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleM1 : ColorRect
{
    private _vectors = new Vector2[] { new(40, 40), new(70, 60), new(80, 120), new(40, -40), new(0, 30) };

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.VectorsAbsolutely(new Vector2(100, 100), _vectors),
            Color.ColorN("gray"));

        DrawMultiline(
            Multiline.VectorsRelatively(new Vector2(300, 300), _vectors),
            Color.ColorN("blue"));
    }
}
```

\
**2) Builder**  
Use the multiline as a builder.  

```cs
DrawMultiline(
    new Multiline()
        .Append<CompositionName>(..)
        .Append<CompositionName>(..),
    <Color>);  
```

pros: low memory footprint  
cons: more lines of code  

_Example M2_ shows how to draw more complex line drawings as one multiline.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleM2 : ColorRect
{
    public override void _Draw()
    {
        var m = new Multiline()
            .AppendRectangle(new Vector2(0,0), 50, 50, 0)
            .AppendDashDottedLine(new Vector2(-60, 0), new Vector2(60, 0))
            .AppendDashDottedLine(new Vector2(0, 60), new Vector2(0, -60));

        DrawMultiline(m.Points, Color.ColorN("black"));
    }
}
```

\
**3) Custom**  
In this case you have full control over collection of points.

```cs
var points = new List<Vector2>(20);
Multiline.Append<CompositionName>(points, ..);
Multiline.Append<CompositionName>(points, ..);
DrawMultiline(points, <Color>);
```

pros: full control  
cons: longer code  


_Example M3_ shows how to use multiline in thread safe manner.  

```cs
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleM3 : ColorRect
{
    private ReaderWriterLockSlim _lock = new();
    private List<Vector2> _points = new();
    
    public override void _Draw()
    {
        ClearPoints();

        AppendMyFigure(_points, new Vector(50, 50));
        AppendMyFigure(_points, new Vector(100, 50));

        _lock.EnterWriteLock();
        var pointsArray = _points.toArray();
        _lock.Release();

        DrawMultiline(pointsArray, Color.ColorN("gray"));
    }

    private void AppendMyFigure(IList<Vector2> points, Vector2 center)
    {
        _lock.EnterWriteLock();
        try
        {
            Multiline.AppendRectangle(_points, ..);
            Multiline.Append(_points, ..);
        }
        finally
        {
            _lock.Release();
        }
    }

    private void ClearPoints()
    {
        _lock.EnterWriteLock();
        _points.Clear();
        _lock.Release();
    }
}
```
