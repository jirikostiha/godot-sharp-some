namespace GodotSharpSome.Drawing2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using static Godot.Mathf;

    public class Multiline
    {
        private const float DefaultArrowAngle = Pi / 14;

        private List<Vector2> _points;

        public Multiline(int capacity)
        {
            _points = new List<Vector2>(capacity);
        }

        public Multiline(List<Vector2> points = null)
        {
            _points = points ?? new List<Vector2>();
        }

        public Multiline AppendCross(Vector2 center, float radius)
        {
            AppendCross(_points, center, radius);
            return this;
        }

        public Multiline AppendCross2(Vector2 center, float outerRadius, float innerRadius)
        {
            AppendCross2(_points, center, outerRadius, innerRadius);
            return this;
        }

        public Multiline AppendArrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendArrow(_points, start, top, headRadius, arrowAngle);
            return this;
        }
        
        public Multiline AppendDoubleArrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendDoubleArrow(_points, start, top, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendSegmentedLine(Vector2 start, Vector2 end, IList<float> distances)
        {
            AppendSegmentedLine(_points, start, end, distances);
            return this;
        }

        public Multiline AppendSegmentedArrow(Vector2 start, Vector2 top, IList<float> distances, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendSegmentedArrow(_points, start, top, distances, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            AppendTriangle(_points, a, b, c);
            return this;
        }

        public Multiline AppendRectangle(Vector2 originVertex, Vector2 directionVertex, float height)
        {
            AppendRectangle(_points, originVertex, directionVertex, height);
            return this;
        }

        public Multiline AppendCandleBar(Vector2 bottom, float bottomOffset, Vector2 top, float topOffset, float bodyHalfWidth)
        {
            AppendCandleBar(_points, bottom, bottomOffset, top, topOffset, bodyHalfWidth);
            return this;
        }

        public Multiline AppendConnection(Vector2 aCenter, float aRadius, Vector2 bCenter, float bRadius, float? aHeadRadius = default, float? bHeadRadius = default)
        {
            AppendLine(_points, aCenter, aRadius, bCenter, bRadius);
            if (aHeadRadius is not null)
                AppendArrowHead(_points, bCenter.DirectionTo(aCenter), aCenter, aHeadRadius.Value);
            if (bHeadRadius is not null)
                AppendArrowHead(_points, aCenter.DirectionTo(bCenter), bCenter, bHeadRadius.Value);

            return this;
        }

        public Multiline Clear()
        {
            _points.Clear();
            return this;
        }

        #region static 

        public static Vector2[] Cross(Vector2 center, float radius)
        {
            var points = new List<Vector2>(2 * 2);
            AppendCross(points, center, radius);
            return points.ToArray();
        }

        public static Vector2[] Cross2(Vector2 center, float outerRadius, float innerRadius)
        {
            var points = new List<Vector2>(2 * 4);
            AppendCross2(points, center, outerRadius, innerRadius);
            return points.ToArray();
        }

        public static Vector2[] Arrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var points = new List<Vector2>(2 * 3);
            AppendArrow(points, start, top, headRadius, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] DoubleArrow(Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var points = new List<Vector2>(2 * 5);
            AppendDoubleArrow(points, start, top, headRadius, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            var points = new List<Vector2>(2 * 3);
            AppendTriangle(points, a, b, c);
            return points.ToArray();
        }

        public static Vector2[] Rectangle(Vector2 center, float halfLength, float halfWidth, float rotationAngle)
        {
            var points = new List<Vector2>(2 * 4);
            AppendRectangle(points, center, halfLength, halfWidth, rotationAngle);
            return points.ToArray();
        }

        public static Vector2[] SegmentedLine(Vector2 start, Vector2 end, int segmentCount)
        {
            var length = (end - start).Length();
            var points = new List<Vector2>(2 + 2 * (segmentCount + 1));
            var distances = Enumerable.Range(0, segmentCount).Select(i => length / segmentCount).ToArray();

            AppendSegmentedLine(points, start, end, distances);
            return points.ToArray();
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, float segmentSize, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var segmentCount = (int)((top - start).Length() / segmentSize);

            return SegmentedArrow(start, top,
                Enumerable.Repeat(segmentSize, segmentCount - 1).ToArray(),
                headRadius, arrowAngle);
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, IList<float> distances, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            var points = new List<Vector2>(6 + 2 * (distances.Count + 1));
            AppendArrow(points, start, top, headRadius, arrowAngle);
            AppendSeparators(points, start, start.DirectionTo(top), distances);

            return points.ToArray();
        }

        public static Vector2[] CandleBar(Vector2 bottom, float bottomOffset, Vector2 top, float topOffset, float bodyHalfWidth)
        {
            var points = new List<Vector2>(6 * 2);
            AppendCandleBar(points, bottom, bottomOffset, top, topOffset, bodyHalfWidth);
            return points.ToArray();
        }

        #endregion

        #region static appending

        public static void AppendCandleBar(IList<Vector2> points, Vector2 bottom, float bottomOffset, Vector2 top, float topOffset, float bodyHalfWidth)
        {
            var dirVector = (top - bottom).Normalized();
            var rectBottom = bottom + dirVector * bottomOffset;
            var rectTop = top - dirVector * topOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendLine(points, bottom, rectBottom);
            AppendLine(points, top, rectTop);
            AppendRectangle(points, rectCenter, (rectCenter - rectBottom).Length(), bodyHalfWidth, dirVector.Angle());
        }

        public static void AppendRectangle(IList<Vector2> points, Vector2 center, float halfLength, float halfWidth, float rotationAngle)
        {
            var vertex1 = center + new Vector2(halfLength, -halfWidth).Rotated(rotationAngle);
            var vertex2 = center + new Vector2(halfLength, halfWidth).Rotated(rotationAngle);
            var vertex3 = center + new Vector2(-halfLength, halfWidth).Rotated(rotationAngle);
            var vertex4 = center + new Vector2(-halfLength, -halfWidth).Rotated(rotationAngle);

            AppendLine(points, vertex1, vertex2);
            AppendLine(points, vertex2, vertex3);
            AppendLine(points, vertex3, vertex4);
            AppendLine(points, vertex4, vertex1);
        }

        public static void AppendTriangle(IList<Vector2> points, Vector2 a, Vector2 b, Vector2 c)
        {
            AppendLine(points, a, b);
            AppendLine(points, b, c);
            AppendLine(points, c, a);
        }

        //todo public static void AppendRectangle(IList<Vector2> points, Vector2 originVertex, Vector2 directionVertex, float height)
        public static void AppendRectangle(IList<Vector2> points, Vector2 leftBottomVertice, Vector2 topRightVertice, float rotationAngle)
        {
            var vertex1 = leftBottomVertice.Rotated(rotationAngle);
            var vertex2 = (leftBottomVertice + new Vector2(topRightVertice.x, 0)).Rotated(rotationAngle);
            var vertex3 = topRightVertice.Rotated(rotationAngle);
            var vertex4 = (topRightVertice + new Vector2(0, topRightVertice.y)).Rotated(rotationAngle);
            
            AppendLine(points, vertex1, vertex2);
            AppendLine(points, vertex2, vertex3);
            AppendLine(points, vertex3, vertex4);
            AppendLine(points, vertex4, vertex1);
        }

        public static void AppendCross(IList<Vector2> points, Vector2 center, float radius)
        {
            AppendLine(points, center.x - radius, center.y, center.x + radius, center.y);
            AppendLine(points, center.x, center.y - radius, center.x, center.y + radius);
        }

        public static void AppendCross2(IList<Vector2> points, Vector2 center, float outerRadius, float innerRadius)
        {
            AppendLine(points, center.x - innerRadius, center.y, center.x - outerRadius, center.y);
            AppendLine(points, center.x + innerRadius, center.y, center.x + outerRadius, center.y);
            AppendLine(points, center.x, center.y - innerRadius, center.x, center.y - outerRadius);
            AppendLine(points, center.x, center.y + innerRadius, center.x, center.y + outerRadius);
        }

        public static void AppendArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
        }

        public static void AppendDoubleArrow(IList<Vector2> points, Vector2 start, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(points, top.DirectionTo(start), start, headRadius, arrowAngle);
        }

        public static void AppendArrowHead(IList<Vector2> points, Vector2 direction, Vector2 top, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            //side line 1
            AppendLine(points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            AppendLine(points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);
        }

        public static void AppendSegmentedLine(IList<Vector2> points, Vector2 start, Vector2 end, IList<float> distances)
        {
            AppendLine(points, start, end);
            AppendSeparators(points, start, start.DirectionTo(end), distances);
        }

        public static void AppendSegmentedArrow(IList<Vector2> points, Vector2 start, Vector2 top, IList<float> distances, float headRadius, float arrowAngle = DefaultArrowAngle)
        {
            AppendArrow(points, start, top, headRadius, arrowAngle);
            AppendSeparators(points, start, start.DirectionTo(top), distances);
        }

        public static void AppendSeparators(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
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
        }

        public static void AppendLine(IList<Vector2> points, Vector2 start, float startOffset, Vector2 end, float endOffset)
        {
            var dirVector = (end - start).Normalized();
            points.Add(start + dirVector * startOffset);
            points.Add(end - dirVector * endOffset);
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
