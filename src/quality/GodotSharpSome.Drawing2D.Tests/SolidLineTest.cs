namespace GodotSharpSome.Drawing2D.Tests;

public class SolidLineTest
{
    [Fact]
    public void AppendLine_ContainsTwoPoints()
    {
        var points = new List<Vector2>();

        new SolidLine().AppendLine(points, Vector2.Zero, Vector2.One);

        Assert.Equal(2, points.Count);
    }
}