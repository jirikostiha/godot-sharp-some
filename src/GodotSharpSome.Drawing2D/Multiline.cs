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
        private const float DefaultArrowAngle = Pi / 14;

        public static Vector2[] Cross(Vector2 center, float radius)
            => AppendCross(new List<Vector2>(2 * 2), center, radius).ToArray();

        public static Vector2[] Cross2(Vector2 center, float outerRadius, float innerRadius)
            => AppendCross2(new List<Vector2>(2 * 4), center, outerRadius, innerRadius).ToArray();

        public static Vector2[] Arrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
            => AppendArrow(new List<Vector2>(2 * 3), start, top, headRadius, arrowAngle).ToArray();

        public static Vector2[] DoubleArrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
            => AppendDoubleArrow(new List<Vector2>(2 * 5), start, top, headRadius, arrowAngle).ToArray();


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
        private static IList<Vector2> AppendArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);

            return points;
        }

        private static IList<Vector2> AppendDoubleArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(points, top.DirectionTo(start), start, headRadius, arrowAngle);

            return points;
        }

        private static IList<Vector2> AppendArrowHead(IList<Vector2> points, Vector2 direction, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            //side line 1
            AppendLine(points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            AppendLine(points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);

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
