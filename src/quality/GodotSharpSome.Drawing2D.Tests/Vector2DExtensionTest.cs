namespace GodotSharpSome.Drawing2D.Tests;

public class Vector2DExtensionTest
{
    [Fact]
    public void MirrorX()
    {
        var mirrored = new Vector2(3, 1).MirrorX(1f);

        Assert.Equal(3f, mirrored.X, 6);
        Assert.Equal(1f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorY()
    {
        var mirrored = new Vector2(1, 3).MirrorY(1f);

        Assert.Equal(1f, mirrored.X, 6);
        Assert.Equal(3f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByDirection_ToVectorParallelToXAxis()
    {
        var mirrored = new Vector2(1, 1).MirrorByDirection(Vector2.Right);

        Assert.Equal(1f, mirrored.X, 6);
        Assert.Equal(-1f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByDirection_ToVectorParallelToYAxis()
    {
        var mirrored = new Vector2(1, 1).MirrorByDirection(Vector2.Down);

        Assert.Equal(-1f, mirrored.X, 6);
        Assert.Equal(1f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByDirection_ToXEqualYLine()
    {
        var mirrored = new Vector2(1, 0).MirrorByDirection(Vector2.One * 3);

        Assert.Equal(0, mirrored.X, 6);
        Assert.Equal(1, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByDirection_ToVectorParallelToXAxisWithOffset()
    {
        var mirrored = new Vector2(2, 2).MirrorByDirection(Vector2.One, Vector2.Right);

        Assert.Equal(2f, mirrored.X, 6);
        Assert.Equal(0f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByDirection_ToVectorParallelToYAxisWithOffset()
    {
        var mirrored = new Vector2(2, 2).MirrorByDirection(Vector2.One, Vector2.Down);

        Assert.Equal(0f, mirrored.X, 6);
        Assert.Equal(2f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByPoints_ToLineParallelToX()
    {
        var mirrored = new Vector2(2, 2).MirrorByPoints(Vector2.One, Vector2.One + Vector2.Right);

        Assert.Equal(2f, mirrored.X, 6);
        Assert.Equal(0f, mirrored.Y, 6);
    }

    [Fact]
    public void MirrorByPoints_ToLineParallelToY()
    {
        var mirrored = new Vector2(2, 2).MirrorByPoints(Vector2.One, Vector2.One + Vector2.Down);

        Assert.Equal(0f, mirrored.X, 6);
        Assert.Equal(2f, mirrored.Y, 6);
    }

    [Fact]
    public void Rotated_AroundOrigin()
    {
        var mirrored = Vector2.Right.Rotated(Mathf.Pi);

        Assert.Equal(-1f, mirrored.X, 6);
        Assert.Equal(0f, mirrored.Y, 6);
    }

    [Fact]
    public void Rotated_AroundPoint()
    {
        var mirrored = Vector2.One.Rotated(Vector2.Right, Mathf.Pi);

        Assert.Equal(1f, mirrored.X, 6);
        Assert.Equal(-1f, mirrored.Y, 6);
    }
}