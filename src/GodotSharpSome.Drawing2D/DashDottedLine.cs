﻿using System.Diagnostics;

namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Immutable straight dashed-dotted line appender.
/// </summary>
[DebuggerDisplay("Dash-Dotted: dash={DashLength}, space={SpaceLength}")]
public sealed class DashDottedLine : IStraightLineAppender
{
    private const float Default_DashLength = 16;
    private const float Default_SpaceLength = 6;

    public static readonly DashDottedLine Default = new();

    public float DashLength { get; }
    public float SpaceLength { get; }

    public DashDottedLine(float spaceLength = Default_SpaceLength, float dashLength = Default_DashLength)
    {
        DashLength = dashLength;
        SpaceLength = spaceLength;
    }

    public int AppendLine(IList<Vector2> points, Vector2 a, Vector2 b) =>
        AppendLinePrivate(points, a, b, DashLength, SpaceLength);

    public static IList<Vector2> AppendLine(IList<Vector2> points, Vector2 a, Vector2 b,
        float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        _ = AppendLinePrivate(points, a, b, dashLength, spaceLength);
        return points;
    }

    private static int AppendLinePrivate(IList<Vector2> points, Vector2 a, Vector2 b,
        float dashLength = Default_DashLength, float spaceLength = Default_SpaceLength)
    {
        var count = SegmentedLineHelper.AdaptSubinterval2((b - a).Length(), spaceLength + 1 + spaceLength, ref dashLength);
        var dir = a.DirectionTo(b);

        for (int i = 0; i < count; i++)
        {
            var segmentStart = a + dir * i * (dashLength + spaceLength + 1 + spaceLength);

            SolidLine.AppendStraightLine(points, segmentStart, segmentStart + dir * dashLength);
            SolidLine.AppendDot(points, segmentStart + dir * (dashLength + spaceLength));
        }

        SolidLine.AppendStraightLine(points, a + dir * count * (dashLength + spaceLength + 1 + spaceLength), b);

        return count;
    }
}