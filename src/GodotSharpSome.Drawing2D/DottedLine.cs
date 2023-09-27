namespace GodotSharpSome.Drawing2D;

public sealed class DottedLine : IStraightLineAppender
{
    private const float Default_SpaceLength = 3;

    public float SpaceLength { get; set; }

    public DottedLine(float spaceLength = Default_SpaceLength)
    {
        SpaceLength = spaceLength;
    }

    public void AppendLine(IList<Vector2> points, Vector2 a, Vector2 b)
        => AppendLine(points, a, b, SpaceLength);

    public static IList<Vector2> AppendLine(IList<Vector2> points, Vector2 a, Vector2 b, float spaceLength = Default_SpaceLength)
    {
        SegmentedLineHelper.AdaptSubinterval((a - b).Length(), 1, ref spaceLength, out int count);
        var dir = a.DirectionTo(b);

        for (int i = 0; i <= count; i++)
            SolidLine.AppendDot(points, a + dir * i * (1 + spaceLength));

        return points;
    }

    public static Vector2[] Line(Vector2 a, Vector2 b, float spaceLength = Default_SpaceLength)
    {
        var count = (b - a).Length() / (1 + spaceLength);
        var points = new List<Vector2>(2 * ((int)count + 1));
        AppendLine(points, a, b, spaceLength);

        return points.ToArray();
    }
}