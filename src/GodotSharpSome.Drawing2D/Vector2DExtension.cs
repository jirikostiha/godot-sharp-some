namespace GodotSharpSome.Drawing2D;

public static class Vector2DExtension
{
    /// <summary>
    /// First normal vector of an input vector. It is the first one in circular direction from (+)x-axis to (+)y-axis.
    /// </summary>
    public static Vector2 Normal1(this Vector2 vector) => -vector.Orthogonal();

    /// <summary>
    /// Second normal vector of an input vector. It is the second one in circular direction from (+)x-axis to (+)y-axis.
    /// </summary>
    public static Vector2 Normal2(this Vector2 vector) => vector.Orthogonal();

    /// <summary>
    /// Rotate vector around given center.
    /// </summary>
    /// <param name="vector"> Vector to rotate. </param>
    /// <param name="center"> Rotation center. </param>
    /// <param name="angle"> Rotation angle. </param>
    /// <returns> Vector with rotated coordinates. </returns>
    public static Vector2 Rotated(this Vector2 vector, Vector2 center, float angle) =>
        (vector - center).Rotated(angle) + center;

    /// <summary>
    /// Mirror vector to a line parallel to x-axis.
    /// </summary>
    /// <param name="vector"> Vector to mirror. </param>
    /// <param name="y"> Y coordinate of a mirror line. </param>
    /// <returns> Vector with mirrored coordinates. </returns>
    public static Vector2 MirrorX(this Vector2 vector, float y) =>
        new(vector.X, vector.Y - 2 * (vector.Y - y));

    /// <summary>
    /// Mirror vector to a line parallel to y-axis.
    /// </summary>
    /// <param name="vector"> Vector to mirror. </param>
    /// <param name="x"> X coordinate of a mirror line. </param>
    /// <returns> Vector with mirrored coordinates. </returns>
    public static Vector2 MirrorY(this Vector2 vector, float x) =>
        new(vector.X - 2 * (vector.X - x), vector.Y);

    /// <summary>
    /// Mirror vector to line determined by origin and direction vector.
    /// </summary>
    /// <param name="vector"> Vector to mirror. </param>
    /// <param name="directionVector"> Direction vector of mirror line. </param>
    /// <returns> Vector with mirrored coordinates. </returns>
    public static Vector2 MirrorByDirection(this Vector2 vector, Vector2 directionVector) =>
        vector.Reflect(directionVector.Normalized());

    /// <summary>
    /// Mirror vector to line determined by one point and direction vector.
    /// </summary>
    /// <param name="vector"> Vector to mirror. </param>
    /// <param name="mirrorPoint"> Point of mirror line. </param>
    /// <param name="directionVector"> Direction vector of mirror line. </param>
    /// <returns> Vector with mirrored coordinates. </returns>
    public static Vector2 MirrorByDirection(this Vector2 vector, Vector2 mirrorPoint, Vector2 directionVector) =>
        (vector - mirrorPoint).Reflect(directionVector.Normalized()) + mirrorPoint;

    /// <summary>
    /// Mirror vector to line determined by two points.
    /// </summary>
    /// <param name="vector"> Vector to mirror.</param>
    /// <param name="mirrorPointA"> First point of mirror line. </param>
    /// <param name="mirrorPointB"> Second point of mirror line. </param>
    /// <returns> Vector with mirrored coordinates. </returns>
    public static Vector2 MirrorByPoints(this Vector2 vector, Vector2 mirrorPointA, Vector2 mirrorPointB)
    {
        var direction = mirrorPointB - mirrorPointA;

        // The two points coincide, so they define no line. Mirror through the point itself.
        return direction == Vector2.Zero
            ? mirrorPointA * 2 - vector
            : vector.MirrorByDirection(mirrorPointA, direction);
    }
}