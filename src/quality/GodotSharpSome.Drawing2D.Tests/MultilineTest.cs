using Godot;

namespace GodotSharpSome.Drawing2D.Tests;

public class MultilineTest
{
    [Fact]
    public void AppendLine_TwoPoints()
    {
        Assert.Equal(2, new Multiline().AppendLine(Vector2.Zero, Vector2.One).Points().Length);
    }

    [Fact]
    public void AppendDot_TwoPoints()
    {
        Assert.Equal(2, new Multiline().AppendDot(Vector2.Zero).Points().Length);
    }

    [Fact]
    public void AppendTriangle_SixPoints()
    {
        Assert.Equal(6, new Multiline().AppendTriangle(Vector2.Zero, Vector2.Up, Vector2.Right).Points().Length);
    }

    [Fact]
    public void AppendRectangle_EithgPoints()
    {
        Assert.Equal(8, new Multiline().AppendRectangle(Vector2.Zero, Vector2.One, 1).Points().Length);
    }

    [Fact]
    public void AppendCross_FourPoints()
    {
        Assert.Equal(4, new Multiline().AppendCross(Vector2.Zero, 1).Points().Length);
    }

    [Fact]
    public void AppendArrow_SixPoints()
    {
        Assert.Equal(6, new Multiline().AppendArrow(Vector2.Zero, Vector2.Up, 4).Points().Length);
    }
}