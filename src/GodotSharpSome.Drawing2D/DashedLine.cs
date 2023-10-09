using System.Diagnostics;

namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Straight dashed line appender.
/// </summary>
[DebuggerDisplay("Dashed: dash={DashLength}, space={SpaceLength}")]
public sealed class DashedLine : IStraightLineAppender
{
    private const float Default_DashLength = 12;
    private const float Default_SpaceLength = 8;

    public float DashLength { get; set; }
    public float SpaceLength { get; set; }

    public DashedLine(float spaceLength = Default_SpaceLength, float dashLength = Default_DashLength)
    {
        DashLength = dashLength;
        SpaceLength = spaceLength;
    }

    public void AppendLine(IList<Vector2> points, Vector2 a, Vector2 b)
        => AppendLine(points, a, b, DashLength, SpaceLength);

    public static IList<Vector2> AppendLine(IList<Vector2> points, Vector2 a, Vector2 b,
        float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        SegmentedLineHelper.AdaptSubinterval2((b - a).Length(), spaceLength, ref dashLength, out int count);
        var dir = a.DirectionTo(b);

        Vector2 segmentStart = a;
        for (int i = 0; i < count; i++)
        {
            segmentStart = a + dir * i * (dashLength + spaceLength);
            points.Add(segmentStart);
            points.Add(segmentStart + dir * dashLength);
        }

        // add tail
        points.Add(segmentStart + dir * (dashLength + spaceLength));
        points.Add(segmentStart + dir * (dashLength + spaceLength) + dir * dashLength);

        return points;
    }

    public static Vector2[] Line(Vector2 a, Vector2 b,
        float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        var count = (b - a).Length() / (1 + spaceLength);
        var points = new List<Vector2>(2 * ((int)count + 1));
        AppendLine(points, a, b, dashLength, spaceLength);

        return points.ToArray();
    }
}