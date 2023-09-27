namespace GodotSharpSome.Drawing2D.Tests;

public class MultilineTest
{
    [Fact]
    public void PenSetup()
    {
        var points = new List<Vector2>();
        var dotted1 = new DottedLine(3);
        var dashed = new DashedLine(10, 5);
        var dotted2 = new DottedLine(5);

        var ml = new Multiline(points, ("dotted1", dotted1), ("dashed1", dashed), ("dotted2", dotted2));

        Assert.Equal(dotted1, ml.Pen);
        Assert.Equal("dotted1", ml.PenKey);

        ml.SetPen(1);

        Assert.Equal(dashed, ml.Pen);
        Assert.Equal("dashed1", ml.PenKey);

        ml.SetPen("dotted2");

        Assert.Equal(dotted2, ml.Pen);
        Assert.Equal("dotted2", ml.PenKey);
    }

    [Fact]
    public void AppendLine_TwoPoints() =>
        Assert.Equal(2, new Multiline().AppendLine(Vector2.Zero, Vector2.One).Points().Length);

    [Fact]
    public void AppendLineByOnePoint()
    {
        var points = new Multiline().AppendLine(Vector2.Zero, Vector2.Right)
            .AppendLine(Vector2.One).Points();

        Assert.Equal(1d, points.ElementAt(2).X, 6);
        Assert.Equal(0d, points.ElementAt(2).Y, 6);
        Assert.Equal(1d, points.Last().X, 6);
        Assert.Equal(1d, points.Last().Y, 6);
    }

    [Fact]
    public void AppendLineFromRef_TwoSpecificPoints()
    {
        var points = new Multiline().AppendLineFromRef(Vector2.Zero, Vector2.Right, Mathf.Pi / 2f, 1).Points();

        Assert.Equal(2, points.Length);
        Assert.Equal(1d, points.Last().X, 6);
        Assert.Equal(1d, points.Last().Y, 6);
    }

    [Fact]
    public void AppendDot_TwoPoints() =>
        Assert.Equal(2, new Multiline().AppendDot(Vector2.Zero).Points().Length);

    [Fact]
    public void AppendDotByDottedLine_TwoPoints()
    {
        var points = new Multiline(("", new DottedLine())).AppendDot(Vector2.Zero).Points();
        Assert.Equal(2, points.Length);

        Assert.Equal(0d, points.First().X, 6);
        Assert.Equal(0d, points.First().Y, 6);
        Assert.Equal(0d, points.Last().X, 6);
        Assert.Equal(1d, points.Last().Y, 6);
    }

    [Fact]
    public void AppendDots_FourPoints() =>
        Assert.Equal(4, new Multiline().AppendDots(new[] { Vector2.Zero, Vector2.One }).Points().Length);

    [Fact]
    public void AppendTriangle_SixPoints() =>
        Assert.Equal(6, new Multiline().AppendTriangle(Vector2.Zero, Vector2.Up, Vector2.Right).Points().Length);

    [Fact]
    public void AppendRectangle_EightPoints() =>
        Assert.Equal(8, new Multiline().AppendRectangle(Vector2.Zero, Vector2.One, 1).Points().Length);

    [Fact]
    public void AppendCross_FourPoints() =>
        Assert.Equal(4, new Multiline().AppendCross(Vector2.Zero, 1).Points().Length);

    [Fact]
    public void AppendCross2_EightPoints() =>
        Assert.Equal(8, new Multiline().AppendCross2(Vector2.Zero, 2, 1).Points().Length);

    [Fact]
    public void AppendArrow_SixPoints() =>
        Assert.Equal(6, new Multiline().AppendArrow(Vector2.Zero, Vector2.Up, 4).Points().Length);

    [Fact]
    public void AppendDoubleArrow_TenPoints() =>
        Assert.Equal(10, new Multiline().AppendDoubleArrow(Vector2.Zero, Vector2.Up, 4).Points().Length);

    [Fact]
    public void AppendVectorsAbsolutely_TwelvePoints() =>
        Assert.Equal(12, new Multiline().AppendVectorsAbsolutely(Vector2.Zero, new Vector2[] { new(1, 0), new(0, 1) }).Points().Length);

    [Fact]
    public void AppendVectorsRelatively_TwelvePoints() =>
        Assert.Equal(12, new Multiline().AppendVectorsRelatively(Vector2.Zero, new Vector2[] { new(1, 0), new(0, 1) }).Points().Length);
}