namespace GodotSharpSome.Drawing2D;

using Godot;
using System.Collections.Generic;

public static class Vector2DExtension
{
    /// <summary>
    /// First normal vector of an input vector. It is the first one in circular direction from (+)x-axis to (+)y-axis.
    /// </summary>
    public static Vector2 Normal1(this Vector2 vector) => new(-vector.Y, vector.X);

    /// <summary>
    /// Second normal vector of an input vector. It is the second one in circular direction from (+)x-axis to (+)y-axis.
    /// </summary>
    public static Vector2 Normal2(this Vector2 vector) => new(vector.Y, -vector.X);

    /// <summary>
    /// Rotate vectors around center [0,0].
    /// </summary>
    /// <param name="vectors"> Vectors. </param>
    /// <param name="angle"> Rotation angle. </param>
    public static IEnumerable<Vector2> Rotate(this IEnumerable<Vector2> vectors, float angle)
    {
        foreach (var vector in vectors)
            yield return vector.Rotated(angle);
    }

    /// <summary>
    /// Rotate vectors around given center.
    /// </summary>
    /// <param name="vectors"> Vectors. </param>
    /// <param name="center"> Rotation center. </param>
    /// <param name="angle"> Rotation angle. </param>
    public static IEnumerable<Vector2> Rotate(this IEnumerable<Vector2> vectors, Vector2 center, float angle)
    {
        foreach (var vector in vectors)
            yield return (vector + center).Rotated(angle) - center;
    }
}