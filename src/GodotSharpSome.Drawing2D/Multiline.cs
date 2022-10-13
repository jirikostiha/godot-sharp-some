namespace GodotSharpSome.Drawing2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using static Godot.Mathf;

    public static class Multiline
    {
        public static Vector2[] Cross(Vector2 center, float radius)
            => AppendCross(new List<Vector2>(2 * 2), center, radius).ToArray();

        public static Vector2[] Cross2(Vector2 center, float outerRadius, float innerRadius)
            => AppendCross2(new List<Vector2>(2 * 4), center, outerRadius, innerRadius).ToArray();
        
        #region append

        private static IList<Vector2> AppendCross(IList<Vector2> points, Vector2 center, float radius)
        {
            AppendLine(points, center.x - radius, center.y, center.x + radius, center.y);
            AppendLine(points, center.x, center.y - radius, center.x, center.y + radius);

            return points;
        }

        private static IList<Vector2> AppendCross2(IList<Vector2> points, Vector2 center, float outerRadius, float innerRadius)
        {
            AppendLine(points, center.x - innerRadius, center.y, center.x - outerRadius, center.y);
            AppendLine(points, center.x + innerRadius, center.y, center.x + outerRadius, center.y);
            AppendLine(points, center.x, center.y - innerRadius, center.x, center.y - outerRadius);
            AppendLine(points, center.x, center.y + innerRadius, center.x, center.y + outerRadius);

            return points;
        }

        private static void AppendLine(IList<Vector2> points, Vector2 start, Vector2 end)
        {
            points.Add(start);
            points.Add(end);
        }

        private static void AppendLine(IList<Vector2> points, float startX, float startY, float endX, float endY)
            => AppendLine(points, new Vector2(startX, startY), new Vector2(endX, endY));

        #endregion
    }
}
