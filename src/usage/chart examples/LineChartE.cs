using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Charts2D;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class LineChartE : LineChart
{
    public override void _Ready()
    {
        var data = new List<Vector2> {

            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.55f, 0.55f),
            new Vector2(0.6f, 0.6f),
            new Vector2(1, 1),
        };

        var s = new Series<Vector2>(o => o.x, o => o.y)
        {
            Objecs = data,
        };

        Series.Add(s);
    }


    //public void DrawChart(Vector2 origin)
    //{
    //    var points = Enumerable.Range(0, 160).Select(i => origin + new Vector2(2 * i, 40 * Sin(0.1f * i)));
    //    DrawMultiline(Multiline.Dots(points), LineColor);
    //}
}
