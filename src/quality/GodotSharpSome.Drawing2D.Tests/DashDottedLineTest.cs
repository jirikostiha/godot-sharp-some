namespace GodotSharpSome.Drawing2D.Tests;

public class DashDottedLineTest
{
    [Fact]
    public void AppendLine_X_ContainsSixPoints()
    {
        var points = new List<Vector2>();

        new DashDottedLine(2, 3).AppendLine(points, Vector2.Zero, Vector2.Right * 11);

        Assert.Equal(6, points.Count);
        Assert.Equal(3, points.ElementAt(1).X);
        Assert.Equal(5, points.ElementAt(2).X);
        Assert.Equal(5, points.ElementAt(3).X);
        Assert.Equal(8, points.ElementAt(4).X);
    }

    [Fact]
    public void AppendLine_Y_ContainsSixPoints()
    {
        var points = new List<Vector2>();

        new DashDottedLine(2, 3).AppendLine(points, Vector2.Zero, Vector2.Down * 11);

        Assert.Equal(6, points.Count);
        Assert.Equal(3, points.ElementAt(1).Y);
        Assert.Equal(8, points.ElementAt(4).Y);
    }
}