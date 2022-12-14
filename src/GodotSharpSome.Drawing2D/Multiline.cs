namespace GodotSharpSome.Drawing2D
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using static Godot.Mathf;

    public class Multiline
    {
        //Default values
        private const float Arrow_HeadAngle = Pi / 14;
        private const float Arrow_HeadRadius = 20;
        private const float DottedLine_SpaceLength = 4;
        private const float DashedLine_DashLength = 12;
        private const float DashedLine_SpaceLength = 8;
        private const float DashDottedLine_DashLength = 16;
        private const float DashDottedLine_SpaceLength = 6;

        private static readonly Vector2 DotVector = Vector2.Down;

        private List<Vector2> _points;

        public Multiline(int capacity)
        {
            _points = new List<Vector2>(capacity);
        }

        public Multiline(List<Vector2>? points = null)
        {
            _points = points ?? new List<Vector2>();
        }

        public Vector2[] Points => _points.ToArray();

        #region instance as builder

        public Multiline AppendDot(Vector2 position)
        {
            AppendDot(_points, position);
            return this;
        }

        public Multiline AppendDots(IEnumerable<Vector2> positions)
        {
            AppendDots(_points, positions);
            return this;
        }

        public Multiline AppendDottedLine(Vector2 start, Vector2 end,
            float spaceLength = DottedLine_SpaceLength)
        {
            AppendDottedLine(_points, start, end, spaceLength);
            return this;
        }

        public Multiline AppendDashedLine(Vector2 start, Vector2 end,
            float dashLength = DashedLine_DashLength, float spaceLength = DashedLine_SpaceLength)
        {
            AppendDashedLine(_points, start, end, dashLength, spaceLength);
            return this;
        }

        public Multiline AppendDashDottedLine(Vector2 start, Vector2 end,
            float dashLength = DashDottedLine_DashLength, float spaceLength = DashDottedLine_SpaceLength)
        {
            AppendDashDottedLine(_points, start, end, dashLength, spaceLength);
            return this;
        }

        public Multiline AppendLine(Vector2 start, Vector2 end)
        {
            AppendLine(_points, start, end);
            return this;
        }

        /// <summary> Append a continuation line from the last point. </summary>
        public Multiline AppendLine(Vector2 end)
        {
            AppendLine(_points, end);
            return this;
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

        public Multiline AppendArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendArrow(_points, start, top, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendDoubleArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendDoubleArrow(_points, start, top, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendSegmentedLine(Vector2 start, Vector2 direction, IList<float> distances)
        {
            AppendSegmentedLine(_points, start, direction, distances);
            return this;
        }

        public Multiline AppendSegmentedArrow(Vector2 start, Vector2 direction, IList<float> distances,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendSegmentedArrow(_points, start, direction, distances, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendVectorsRelatively(Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            AppendVectorsRelatively(_points, zero, vectors, arrowAngle);
            return this;
        }

        public Multiline AppendVectorsAbsolutely(Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            AppendVectorsAbsolutely(_points, zero, vectors, arrowAngle);
            return this;
        }

        public Multiline AppendAxes(Vector2 origin, Vector2 xDirection, float xUnitLength, int xUnitCount, float yUnitLength, int yUnitCount,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendAxes(_points, origin, xDirection, xUnitLength, xUnitCount, yUnitLength, yUnitCount, headRadius, arrowAngle);
            return this;
        }

        public Multiline AppendTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            AppendTriangle(_points, a, b, c);
            return this;
        }

        public Multiline AppendRectangle(Vector2 center, float halfLength, float halfWidth, float rotationAngle)
        {
            AppendRectangle(_points, center, halfLength, halfWidth, rotationAngle);
            return this;
        }

        public Multiline AppendRectangle(Vector2 vertex1, Vector2 vertex2, float height)
        {
            AppendRectangle(_points, vertex1, vertex2, height);
            return this;
        }

        public Multiline AppendRegularConvexPolygon(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            AppendRegularConvexPolygon(_points, center, radius, verticesCount, rotationAngle);
            return this;
        }

        public Multiline AppendCandlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            AppendCandlestick(_points, low, lowOffset, high, highOffset, halfWidth);
            return this;
        }

        public Multiline AppendCrossedCandlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            AppendCrossedCandlestick(_points, low, lowOffset, high, highOffset, halfWidth, upDirrection);
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

        #endregion

        #region static 

        public static Vector2[] Dot(Vector2 position)
        {
            return new Vector2[2] { position, position + DotVector };
        }

        public static Vector2[] Dots(IList<Vector2> positions)
        {
            var points = new Vector2[2 * positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                points[2*i] = positions[i];
                points[2*i+1] = positions[i] + Vector2.Down;
            }
            return points;
        }

        public static Vector2[] DottedLine(Vector2 start, Vector2 end,
            float spaceLength = DottedLine_SpaceLength)
        {
            var count = (end - start).Length() / (1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDottedLine(points, start, end, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] DashedLine(Vector2 start, Vector2 end,
            float dashLength = DashedLine_DashLength, float spaceLength = DashedLine_SpaceLength)
        {
            var count = (end - start).Length() / (1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDashedLine(points, start, end, dashLength, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] DashDottedLine(Vector2 start, Vector2 end,
            float dashLength = DashDottedLine_DashLength, float spaceLength = DashDottedLine_SpaceLength)
        {
            var count = (end - start).Length() / (dashLength + spaceLength + 1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDashDottedLine(points, start, end, dashLength, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] Line(Vector2 start, Vector2 end)
        {
            return new Vector2[2] { start, end };
        }

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

        public static Vector2[] Arrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(2 * 3);
            AppendArrow(points, start, top, headRadius, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] DoubleArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(2 * 5);
            AppendDoubleArrow(points, start, top, headRadius, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] SegmentedLine(Vector2 start, Vector2 end, int segmentCount)
        {
            var segmentLength = (end - start).Length() / segmentCount;
            var points = new List<Vector2>(2 + 2 * (segmentCount + 1));
            var distances = Enumerable.Range(0, segmentCount + 1).Select(i => Min(1, i) * segmentLength).ToArray();

            AppendSegmentedLine(points, start, start.DirectionTo(end), distances);
            return points.ToArray();
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, float segmentLength,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var segmentCount = (int)((top - start).Length() / segmentLength);

            return SegmentedArrow(start, start.DirectionTo(top),
                Enumerable.Repeat(segmentLength, segmentCount - 1).ToArray(),
                headRadius, arrowAngle);
        }

        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 direction, IList<float> distances,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(6 + 2 * (distances.Count + 1));
            AppendSegmentedArrow(points, start, direction, distances, headRadius, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] VectorsRelatively(Vector2 zero, IList<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(2 * 3 * vectors.Count);
            AppendVectorsRelatively(points, zero, vectors, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] VectorsAbsolutely(Vector2 zero, IList<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(2 * 3 * vectors.Count);
            AppendVectorsAbsolutely(points, zero, vectors, arrowAngle);
            return points.ToArray();
        }

        public static Vector2[] Axes(Vector2 origin, Vector2 xDirection, float xUnitLength, int xUnitCount, float yUnitLength, int yUnitCount,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var points = new List<Vector2>(2 * 3 * 2);
            AppendAxes(points, origin, xDirection, xUnitLength, xUnitCount, yUnitLength, yUnitCount, headRadius, arrowAngle);
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

        public static Vector2[] Rectangle(Vector2 vertex1, Vector2 vertex2, float height)
        {
            var points = new List<Vector2>(2 * 4);
            AppendRectangle(points, vertex1, vertex2, height);
            return points.ToArray();
        }

        public static IEnumerable<Vector2> RegularConvexPolygonVertices(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var segmentAngle = 2 * Pi / verticesCount;
            float angle;
            for (int i = 0; i < verticesCount; i++)
            {
                angle = rotationAngle + segmentAngle * i;
                yield return new Vector2(radius * Cos(angle) + center.x, radius * Sin(angle) + center.y);
            }
        }

        public static Vector2[] RegularConvexPolygon(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var points = new List<Vector2>((verticesCount + 1) * 2);
            AppendRegularConvexPolygon(points, center, radius, verticesCount, rotationAngle);
            return points.ToArray();
        }

        public static Vector2[] Candlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            var points = new List<Vector2>(6 * 2);
            AppendCandlestick(points, low, lowOffset, high, highOffset, halfWidth);
            return points.ToArray();
        }

        public static Vector2[] CrossedCandlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            var points = new List<Vector2>(7 * 2);
            AppendCrossedCandlestick(points, low, lowOffset, high, highOffset, halfWidth, upDirrection);
            return points.ToArray();
        }

        #endregion

        #region static appending

        public static void AppendDot(IList<Vector2> points, Vector2 position)
        {
            points.Add(position);
            points.Add(position + DotVector);
        }

        public static void AppendDots(IList<Vector2> points, IEnumerable<Vector2> positions)
        {
            foreach (var position in positions)
            {
                points.Add(position);
                points.Add(position + Vector2.Down);
            }
        }

        public static void AppendDottedLine(IList<Vector2> points, Vector2 start, Vector2 end,
            float spaceLength = DottedLine_SpaceLength)
        {
            AdaptSubinterval((end - start).Length(), 1, ref spaceLength, out int count);
            var dir = start.DirectionTo(end);

            for (int i = 0; i <= count; i++)
                AppendDot(points, start + dir * i * (1 + spaceLength));
        }

        public static void AppendDashedLine(IList<Vector2> points, Vector2 start, Vector2 end,
            float dashLength = DashedLine_DashLength, float spaceLength = DashedLine_SpaceLength)
        {
            AdaptSubinterval((end - start).Length(), spaceLength, ref dashLength, out int count);
            var dir = start.DirectionTo(end);

            for (int i = 0; i < count; i++)
            {
                var segmentStart = start + dir * i * (dashLength + spaceLength);
                AppendLine(points, segmentStart, segmentStart + dir * dashLength);
            }
        }

        public static void AppendDashDottedLine(IList<Vector2> points, Vector2 start, Vector2 end,
            float dashLength = DashDottedLine_DashLength, float spaceLength = DashDottedLine_SpaceLength)
        {
            AdaptSubinterval((end - start).Length(), spaceLength + 1 + spaceLength, ref dashLength, out int count);
            var dir = start.DirectionTo(end);

            for (int i = 0; i < count - 1; i++)
            {
                var segmentStart = start + dir * i * (dashLength + spaceLength + 1 + spaceLength);
                AppendLine(points, segmentStart, segmentStart + dir * dashLength);
                AppendDot(points, segmentStart + dir * (dashLength + spaceLength));
            }

            AppendLine(points, start + dir * (count - 1) * (dashLength + spaceLength + 1 + spaceLength), end);
        }

        private static void AdaptSubinterval(float totalLength, float fixedInterval, ref float adaptingInterval, out int count)
        {
            float segmentLength = adaptingInterval + fixedInterval;
            float segmentCount = totalLength / segmentLength;
            count = RoundToInt(segmentCount);
            float adaptedSegmentLength = (totalLength + fixedInterval) / count;
            adaptingInterval = adaptedSegmentLength - fixedInterval;
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

        /// <summary> Append a continuation line from the last point. </summary>
        public static void AppendLine(IList<Vector2> points, Vector2 end)
        {
            points.Add(points[points.Count - 1]);
            points.Add(end);
        }

        public static void AppendLine(IList<Vector2> points, float startX, float startY, float endX, float endY)
            => AppendLine(points, new Vector2(startX, startY), new Vector2(endX, endY));

        /// <summary> Append a continuation line from the last point. </summary>
        public static void AppendLine(IList<Vector2> points, float endX, float endY)
            => AppendLine(points, new Vector2(endX, endY));

        private static void AppendLine(Vector2[] points, int index, Vector2 start, Vector2 end)
        {
            points[index] = start;
            points[index + 1] = end;
        }

        public static void AppendSeparators(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();
            var normal = new Vector2(dir.y, -dir.x);

            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                AppendLine(points,
                    (start + dir * distSum) + normal * 2,
                    (start + dir * distSum) - normal * 2);
            }
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

        public static void AppendArrow(IList<Vector2> points, Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
        }

        public static void AppendDoubleArrow(IList<Vector2> points, Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            AppendLine(points, start, top);
            AppendArrowHead(points, start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(points, top.DirectionTo(start), start, headRadius, arrowAngle);
        }

        public static void AppendArrowHead(IList<Vector2> points, Vector2 direction, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            //side line 1
            AppendLine(points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            AppendLine(points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);
        }

        public static void AppendSegmentedLine(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();
            AppendLine(points, start, start + dir * distances.Sum());
            AppendSeparators(points, start, dir, distances);
        }

        public static void AppendSegmentedArrow(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var dir = direction.Normalized();
            AppendArrow(points, start, start + dir * (distances.Sum() + 2 * headRadius), headRadius, arrowAngle);
            AppendSeparators(points, start, dir, distances);
        }

        public static void AppendVectorsRelatively(IList<Vector2> points, Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            var offset = zero;
            foreach (var vector in vectors)
                AppendArrow(points, offset, offset += vector, Clamp(vector.Length() / 4f, 14, 20), arrowAngle);
        }

        public static void AppendVectorsAbsolutely(IList<Vector2> points, Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            foreach (var vector in vectors)
                AppendArrow(points, zero, zero + vector, Clamp(vector.Length() / 4f, 14, 20), arrowAngle);
        }

        public static void AppendAxes(IList<Vector2> points, Vector2 origin, Vector2 xDirection, float xUnitLength, int xUnitCount, float yUnitLength, int yUnitCount,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var xDistances = Enumerable.Range(0, xUnitCount).Select(i => xUnitLength).ToArray();
            var yDistances = Enumerable.Range(0, yUnitCount).Select(i => yUnitLength).ToArray();

            AppendSegmentedArrow(points, origin, xDirection, xDistances, headRadius, arrowAngle);
            var yDirection = new Vector2(-xDirection.y, xDirection.x);
            AppendSegmentedArrow(points, origin, yDirection, yDistances, headRadius, arrowAngle);
        }

        /// <summary>
        /// Append rectangle by center, half sizes and orientation.
        /// </summary>
        /// <param name="points"> Existing collection of points. </param>
        /// <param name="center"> Rectangle center. </param>
        /// <param name="halfLength"> Half size of rectangle length. </param>
        /// <param name="halfWidth"> Half size of rectangle width. </param>
        /// <param name="rotationAngle"> Orientation in radians. </param>
        public static void AppendRectangle(IList<Vector2> points, Vector2 center, float halfLength, float halfWidth, float rotationAngle)
        {
            var vertex1 = center + new Vector2(-halfLength, -halfWidth).Rotated(rotationAngle);
            var vertex2 = center + new Vector2(-halfLength, halfWidth).Rotated(rotationAngle);

            AppendRectangle(points, vertex1, vertex2, 2 * halfLength);
        }

        /// <summary>
        /// Append rectangle by two vertices and height.
        /// </summary>
        /// <param name="points"> Existing collection of points. </param>
        /// <param name="vertex1"> Primary vertex. </param>
        /// <param name="vertex2"> Secondary vertex setting up base side of the rectangle. </param>
        /// <param name="height"> Distance of other side from base side. Positive is on left side of direction vertex/vector. </param>
        public static void AppendRectangle(IList<Vector2> points, Vector2 vertex1, Vector2 vertex2, float height)
        {
            var dirVector = vertex1.DirectionTo(vertex2);
            var normalVector = new Vector2(dirVector.y, -dirVector.x);
            var vertex3 = vertex2 + normalVector * height;
            var vertex4 = vertex1 + normalVector * height;

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

        public static void AppendRegularConvexPolygon(IList<Vector2> points, Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var vertices = RegularConvexPolygonVertices(center, radius, verticesCount, rotationAngle)
                .ToArray();

            for (int i = 1; i < vertices.Length; i++)
                AppendLine(points, vertices[i - 1], vertices[i]);

            // closing line
            AppendLine(points, vertices[verticesCount - 1], vertices[0]);
        }

        public static void AppendCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            var dirVector = low.DirectionTo(high);
            var rectBottom = low + dirVector * lowOffset;
            var rectTop = high - dirVector * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
        }

        public static void AppendCrossedCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            var dirVector = low.DirectionTo(high);
            var rectBottom = low + dirVector * lowOffset;
            var rectTop = high - dirVector * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
            if (upDirrection)
            {
                AppendLine(points,
                    new Vector2(rectBottom.x - halfWidth, rectBottom.y),
                    new Vector2(rectTop.x + halfWidth, rectTop.y));
            }
            else
                AppendLine(points,
                    new Vector2(rectBottom.x + halfWidth, rectBottom.y),
                    new Vector2(rectTop.x - halfWidth, rectTop.y));
        }

        private static void AppendCandlestick(IList<Vector2> points, Vector2 bottom, Vector2 top, Vector2 rectBottom, Vector2 rectCenter, Vector2 rectTop, float bodyHalfWidth)
        {
            AppendLine(points, bottom, rectBottom);
            AppendLine(points, top, rectTop);
            AppendRectangle(points, rectCenter, (rectCenter - rectBottom).Length(), bodyHalfWidth, bottom.DirectionTo(top).Angle());
        }

        #endregion
    }
}
