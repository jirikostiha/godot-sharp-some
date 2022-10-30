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

        public static Vector2[] Rectangle(Vector2 center, float halfLength, float halfWidth, float angle)
        => AppendRectangle(new List<Vector2>(2 * 4), center, halfLength, halfWidth, angle).ToArray();

        public static Vector2[] SegmentedLine(Vector2 start, Vector2 end, int segmentCount)
        {
            var length = (end - start).Length();
            var points = new List<Vector2>(2 + 2 * (segmentCount + 1));
            var distances = Enumerable.Range(0, segmentCount).Select(i => length / segmentCount).ToArray();
            
            return AppendSegmentedLine(points, start, end, distances).ToArray();
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, float segmentSize, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var segmentCount = (int)((top - start).Length() / segmentSize);
            
            return SegmentedArrow(start, top,
                Enumerable.Repeat(segmentSize, segmentCount - 1).ToArray(),
                headRadius, arrowAngle)
                .ToArray();
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, IList<float> distances, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var points = new List<Vector2>(6 + 2 * (distances.Count + 1));
            AppendArrow(points, start, top, headRadius, arrowAngle);
            AppendSeparators(points, start, start.DirectionTo(top), distances);

            return points.ToArray();
        }

        public static Vector2[] CandleBar(Vector2 bottom, float bottomOffset, Vector2 top, float topOffset, float bodyHalfWidth)
            => AppendCandleBar(new List<Vector2>(2 * 6), bottom, bottomOffset, top, topOffset, bodyHalfWidth).ToArray();

        #region append

        public static IList<Vector2> AppendCandleBar(IList<Vector2> points, Vector2 bottom, float bottomOffset, Vector2 top, float topOffset, float bodyHalfWidth)
        {
            var dirVector = (top - bottom).Normalized();
            var rectBottom = bottom + dirVector * bottomOffset;
            var rectTop = top - dirVector * topOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendLine(points, bottom, rectBottom);
            AppendLine(points, top, rectTop);
            AppendRectangle(points, rectCenter, (rectCenter - rectBottom).Length(), bodyHalfWidth, dirVector.Angle());

            return points;
        }

        public static IList<Vector2> AppendRectangle(IList<Vector2> points, Vector2 center, float halfLength, float halfWidth, float angle)
        {
            var vertice1 = center + new Vector2(halfLength, -halfWidth).Rotated(angle);
            var vertice2 = center + new Vector2(halfLength, halfWidth).Rotated(angle);
            var vertice3 = center + new Vector2(-halfLength, halfWidth).Rotated(angle);
            var vertice4 = center + new Vector2(-halfLength, -halfWidth).Rotated(angle);

            AppendLine(points, vertice1, vertice2);
            AppendLine(points, vertice2, vertice3);
            AppendLine(points, vertice3, vertice4);
            AppendLine(points, vertice4, vertice1);

            return points;
        }


        public static IList<Vector2> AppendRectangle(IList<Vector2> points, Vector2 leftBottomVertice, Vector2 topRightVertice, float angle)
        {
            var vertice1 = leftBottomVertice.Rotated(angle);
            var vertice2 = (leftBottomVertice + new Vector2(topRightVertice.x, 0)).Rotated(angle);
            var vertice3 = topRightVertice.Rotated(angle);
            var vertice4 = (topRightVertice + new Vector2(0, topRightVertice.y)).Rotated(angle);
            
            AppendLine(points, vertice1, vertice2);
            AppendLine(points, vertice2, vertice3);
            AppendLine(points, vertice3, vertice4);
            AppendLine(points, vertice4, vertice1);

            return points;
        }

        public static IList<Vector2> AppendCross(IList<Vector2> points, Vector2 center, float radius)
        {
            AppendLine(points, center.x - radius, center.y, center.x + radius, center.y);
            AppendLine(points, center.x, center.y - radius, center.x, center.y + radius);

            return points;
        }

        public static IList<Vector2> AppendCross2(IList<Vector2> points, Vector2 center, float outerRadius, float innerRadius)
        {
            AppendLine(points, center.x - innerRadius, center.y, center.x - outerRadius, center.y);
            AppendLine(points, center.x + innerRadius, center.y, center.x + outerRadius, center.y);
            AppendLine(points, center.x, center.y - innerRadius, center.x, center.y - outerRadius);
            AppendLine(points, center.x, center.y + innerRadius, center.x, center.y + outerRadius);

            return points;
        }

        public static IList<Vector2> AppendArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);

            return points;
        }

        public static IList<Vector2> AppendDoubleArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(points, top.DirectionTo(start), start, headRadius, arrowAngle);

            return points;
        }

        public static IList<Vector2> AppendArrowHead(IList<Vector2> points, Vector2 direction, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            //side line 1
            AppendLine(points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            AppendLine(points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);

            return points;
        }

        public static IList<Vector2> AppendSegmentedLine(IList<Vector2> points, Vector2 start, Vector2 end, IList<float> distances)
        {
            AppendLine(points, start, end);
            AppendSeparators(points, start, start.DirectionTo(end), distances);

            return points;
        }

        public static IList<Vector2> AppendSeparators(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();
            var normal = new Vector2(dir.y, -dir.x);

            // zero line
            AppendLine(points,
                (start + dir) + normal * 3,
                (start + dir) - normal * 3);

            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                AppendLine(points,
                    (start + dir * distSum) + normal * 2,
                    (start + dir * distSum) - normal * 2);
            }

            return points;
        }

        public static void AppendLine(IList<Vector2> points, Vector2 start, Vector2 end)
        {
            points.Add(start);
            points.Add(end);
        }

        public static void AppendLine(IList<Vector2> points, float startX, float startY, float endX, float endY)
            => AppendLine(points, new Vector2(startX, startY), new Vector2(endX, endY));

        #endregion
    }
}
