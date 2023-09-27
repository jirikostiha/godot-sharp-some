namespace GodotSharpSome.Drawing2D;

public sealed class DashDottedLine : IStraightLineAppender
{
    private const float Default_DashLength = 16;
    private const float Default_SpaceLength = 6;

    public float DashLength { get; set; }
    public float SpaceLength { get; set; }

    public DashDottedLine(float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        DashLength = dashLength;
        SpaceLength = spaceLength;
    }

    public void AppendLine(IList<Vector2> points, Vector2 a, Vector2 b)
        => AppendLine(points, a, b, DashLength, SpaceLength);

    public static IList<Vector2> AppendLine(IList<Vector2> points, Vector2 a, Vector2 b,
        float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        SegmentedLineHelper.AdaptSubinterval2((b - a).Length(), spaceLength + 1 + spaceLength, ref dashLength, out int count);
        var dir = a.DirectionTo(b);

        for (int i = 0; i < count; i++)
        {
            var segmentStart = a + dir * i * (dashLength + spaceLength + 1 + spaceLength);

            SolidLine.AppendStraightLine(points, segmentStart, segmentStart + dir * dashLength);
            SolidLine.AppendDot(points, segmentStart + dir * (dashLength + spaceLength));
        }

        SolidLine.AppendStraightLine(points, a + dir * count * (dashLength + spaceLength + 1 + spaceLength), b);

        return points;
    }
}