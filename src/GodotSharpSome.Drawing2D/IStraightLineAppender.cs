namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Append line segment.
/// </summary>
public interface IStraightLineAppender //or pen or builder
{
    void AppendLine(IList<Vector2> points, Vector2 a, Vector2 b);
}