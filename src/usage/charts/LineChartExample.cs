using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Charts2D;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public partial class LineChartExample : Chart
{
    public override void _Ready()
    {
        var s1 = new LineSeries()
        {
            Data = TestData().ToList(),
            Color = new("red")
        };

        Series.Add(s1);

        var s2 = new LineSeries()
        {
            Data = SinData().ToList(),
            Color = new("gray")
        };

        Series.Add(s2);
    }

    //[next] by smath
    private IEnumerable<Vector2> SinData()
    {
        var d = 0.01f;
        for (int i = 0; i < 100; i++)
            yield return new Vector2(i * d, 0.5f + 0.5f * Sin(i * d * 2 * Pi));
    }

    private IEnumerable<Vector2> TestData()
    {
        yield return new Vector2(0, 0);
        yield return new Vector2(1, 0);
        yield return new Vector2(0, 1);
        yield return new Vector2(0.4f, 0.6f);
        yield return new Vector2(0.4f, 0.4f);
        yield return new Vector2(0.5f, 0.5f);
        yield return new Vector2(0.6f, 0.6f);
        yield return new Vector2(0.6f, 0.4f);
        yield return new Vector2(1, 1);
    }
}
