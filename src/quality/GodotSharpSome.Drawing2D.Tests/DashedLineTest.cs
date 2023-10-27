namespace GodotSharpSome.Drawing2D.Tests;

public class DashedLineTest
{
    [Fact]
    public void AppendLine_X_ContainsFourPoints()
    {
        var points = new List<Vector2>();

        new DashedLine(2, 4).AppendLine(points, Vector2.Zero, Vector2.Right * 10);

        Assert.Equal(4, points.Count);
        Assert.Equal(4, points.ElementAt(1).X);
        Assert.Equal(6, points.ElementAt(2).X);
    }

    [Fact]
    public void AppendLine_XAdapted_ContainsSixPoints()
    {
        var points = new List<Vector2>();

        new DashedLine(2, 3).AppendLine(points, Vector2.Zero, Vector2.Right * 12);

        Assert.Equal(6, points.Count);
        Assert.Equal(12, points.ElementAt(5).X);
    }

    [Fact]
    public void AppendLine_Y_ContainsFourPoints()
    {
        var points = new List<Vector2>();

        new DashedLine(2, 4).AppendLine(points, Vector2.Zero, Vector2.Down * 10);

        Assert.Equal(4, points.Count);
        Assert.Equal(4, points.ElementAt(1).Y);
        Assert.Equal(6, points.ElementAt(2).Y);
    }
}