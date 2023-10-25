using System.Diagnostics;

namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Immutable straight solid line appender.
/// </summary>
[DebuggerDisplay("Solid")]
public sealed class SolidLine : IStraightLineAppender
{
    private static readonly Vector2 DefaultDotVector = Vector2.Down;

    public static readonly SolidLine Default = new();

    public Vector2 DotVector { get; } = DefaultDotVector;

    public void AppendLine(IList<Vector2> points, Vector2 a, Vector2 b)
        => AppendStraightLine(points, a, b);

    public static IList<Vector2> AppendStraightLine(IList<Vector2> points, Vector2 a, Vector2 b)
    {
        points.Add(a);
        points.Add(b);

        return points;
    }

    public static Vector2[] AppendStraightLine(Vector2[] points, int index, Vector2 start, Vector2 end)
    {
        points[index] = start;
        points[index + 1] = end;

        return points;
    }

    public static Vector2[] Line(Vector2 a, Vector2 b)
        => new Vector2[2] { a, b };

    public static IList<Vector2> AppendDot(IList<Vector2> points, Vector2 position)
    {
        points.Add(position);
        points.Add(position + DefaultDotVector);

        return points;
    }

    public static IList<Vector2> AppendDots(IList<Vector2> points, IEnumerable<Vector2> positions)
    {
        foreach (var position in positions)
        {
            points.Add(position);
            points.Add(position + DefaultDotVector);
        }

        return points;
    }

    public static Vector2[] Dot(Vector2 position)
    {
        return new Vector2[2] { position, position + DefaultDotVector };
    }

    public static Vector2[] Dots(IList<Vector2> positions)
    {
        var points = new Vector2[2 * positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            points[2 * i] = positions[i];
            points[2 * i + 1] = positions[i] + DefaultDotVector;
        }
        return points;
    }
}