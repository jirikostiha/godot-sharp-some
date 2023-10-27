using System.Diagnostics;

namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Immutable straight dotted line appender.
/// </summary>
[DebuggerDisplay("Dotted: space={SpaceLength}")]
public sealed class DottedLine : IStraightLineAppender
{
    private const float Default_SpaceLength = 3;

    public static readonly DottedLine Default = new();

    public float SpaceLength { get; }

    public DottedLine(float spaceLength = Default_SpaceLength)
    {
        SpaceLength = spaceLength;
    }

    public int AppendLine(IList<Vector2> points, Vector2 a, Vector2 b)
        => AppendLinePrivate(points, a, b, SpaceLength);

    public static IList<Vector2> AppendLine(IList<Vector2> points, Vector2 a, Vector2 b,
        float spaceLength = Default_SpaceLength)
    {
        _ = AppendLinePrivate(points, a, b, spaceLength);

        return points;
    }

    private static int AppendLinePrivate(IList<Vector2> points, Vector2 a, Vector2 b,
        float spaceLength = Default_SpaceLength)
    {
        var count = SegmentedLineHelper.AdaptSubinterval((a - b).Length(), 1, ref spaceLength);
        var dir = a.DirectionTo(b);

        for (int i = 0; i <= count; i++)
            SolidLine.AppendDot(points, a + dir * i * (1 + spaceLength));

        return count;
    }

    public static Vector2[] Line(Vector2 a, Vector2 b,
        float spaceLength = Default_SpaceLength)
    {
        var count = (b - a).Length() / (1 + spaceLength);
        var points = new List<Vector2>(2 * ((int)count + 1));
        AppendLinePrivate(points, a, b, spaceLength);

        return points.ToArray();
    }
}