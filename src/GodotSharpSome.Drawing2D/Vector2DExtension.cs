namespace GodotSharpSome.Drawing2D
{
    using System.Collections.Generic;
    using Godot;

    public static class Vector2DExtension
    {
        //todo to 1st or 2nd normal
        /// <summary>
        /// Get left-handed normal vector to input vector.
        /// </summary>
        public static Vector2 LeftNormal(this Vector2 vector) => new(-vector.Y, vector.X);

        //todo to 1st or 2nd normal
        /// <summary>
        /// Get right-handed normal vector to input vector.
        /// </summary>
        public static Vector2 RightNormal(this Vector2 vector) => new (vector.Y, -vector.X);
      
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
}
