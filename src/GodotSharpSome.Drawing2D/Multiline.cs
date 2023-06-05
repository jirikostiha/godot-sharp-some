namespace GodotSharpSome.Drawing2D
{
    using Godot;
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly IList<Vector2> _points;

        public Multiline(int capacity)
        {
            _points = new List<Vector2>(capacity);
        }

        public Multiline(IList<Vector2>? points = null)
        {
            _points = points ?? new List<Vector2>();
        }

        public Vector2[] Points() => _points.ToArray();

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
            AppendDottedLineAdapted(_points, start, end, spaceLength);
            return this;
        }

        public Multiline AppendDashedLine(Vector2 start, Vector2 end,
            float dashLength = DashedLine_DashLength, float spaceLength = DashedLine_SpaceLength)
        {
            AppendDashedLineAdapted(_points, start, end, dashLength, spaceLength);
            return this;
        }

        public Multiline AppendDashDottedLine(Vector2 start, Vector2 end,
            float dashLength = DashDottedLine_DashLength, float spaceLength = DashDottedLine_SpaceLength)
        {
            AppendDashDottedLineAdapted(_points, start, end, dashLength, spaceLength);
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

        /// <summary> Append a continuation line from <paramref name="start"/> point to <paramref name="end"/> point
        /// with <paramref name="startOffset"/> shift from the start point and <paramref name="endOffset"/> shift from the end.
        /// </summary>
        public Multiline AppendLine(Vector2 start, float startOffset, Vector2 end, float endOffset)
        {
            AppendLine(_points, start, startOffset, end, endOffset);
            return this;
        }

        public Multiline AppendLineFromRef(float angle, float length)
        {
            AppendLineFromRef(_points, angle, length);
            return this;
        }

        public Multiline AppendLineFromRef(Vector2 refPoint, Vector2 start, float angle, float length, float offset = 0)
        {
            AppendLineFromRef(_points, refPoint, start, angle, length, offset);
            return this;
        }

        /// <summary> Append a continuation line parallel to line from given reference points. </summary>
        public Multiline AppendParallelLine(Vector2 refStart, Vector2 refEnd, float distance, float startOffset = 0, float length = 0)
        {
            AppendParallelLine(_points, refStart, refEnd, distance, startOffset, length);
            return this;
        }

        /// <summary> Append a continuation line parallel to line from two given points. </summary>
        public Multiline AppendParallelLines(Vector2 start, Vector2 end, float distance, int count)
        {
            AppendParallelLines(_points, start, end, distance, count);
            return this;
        }

        public Multiline AppendParallelLines(Vector2 refStart, Vector2 refEnd, IList<float> distances)
        {
            AppendParallelLines(_points, refStart, refEnd, distances);
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

        //todo move to charts
        public Multiline AppendSegmentedLine(Vector2 start, Vector2 direction, IList<float> distances)
        {
            AppendSegmentedLine(_points, start, direction, distances);
            return this;
        }

        //todo move to charts
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

        //todo move to charts
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

        //todo move to graphs
        public Multiline AppendConnection(Vector2 aCenter, float aRadius, Vector2 bCenter, float bRadius, float? aHeadRadius = default, float? bHeadRadius = default)
        {
            AppendLine(_points, aCenter, aRadius, bCenter, bRadius);
            if (aHeadRadius is not null)
                AppendArrowHead(_points, bCenter.DirectionTo(aCenter), aCenter, aHeadRadius.Value);
            if (bHeadRadius is not null)
                AppendArrowHead(_points, aCenter.DirectionTo(bCenter), bCenter, bHeadRadius.Value);

            return this;
        }

        public Multiline RemoveLast()
        {
            if (_points.Count > 1)
            {
                _points.RemoveAt(_points.Count - 1);
                _points.RemoveAt(_points.Count - 1);
            }
            return this;
        }

        public Multiline Clear()
        {
            _points.Clear();
            return this;
        }

        #endregion instance as builder

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
                points[2 * i] = positions[i];
                points[2 * i + 1] = positions[i] + Vector2.Down;
            }
            return points;
        }

        public static Vector2[] DottedLine(Vector2 start, Vector2 end,
            float spaceLength = DottedLine_SpaceLength)
        {
            var count = (end - start).Length() / (1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDottedLineAdapted(points, start, end, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] DashedLine(Vector2 start, Vector2 end,
            float dashLength = DashedLine_DashLength, float spaceLength = DashedLine_SpaceLength)
        {
            var count = (end - start).Length() / (1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDashedLineAdapted(points, start, end, dashLength, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] DashDottedLine(Vector2 start, Vector2 end,
            float dashLength = DashDottedLine_DashLength, float spaceLength = DashDottedLine_SpaceLength)
        {
            var count = (end - start).Length() / (dashLength + spaceLength + 1 + spaceLength);
            var points = new List<Vector2>(2 * ((int)count + 1));
            AppendDashDottedLineAdapted(points, start, end, dashLength, spaceLength);
            return points.ToArray();
        }

        public static Vector2[] Line(Vector2 start, Vector2 end)
        {
            return new Vector2[2] { start, end };
        }

        public static Vector2[] Line(Vector2 start, float startOffset, Vector2 end, float endOffset)
        {
            var points = new List<Vector2>(2);
            AppendLine(points, start, startOffset, end, endOffset);
            return points.ToArray();
        }

        /// <summary> Append a continuation line by angle and length and offset relative to the reference points. </summary>
        public static Vector2[] LineFromRef(Vector2 refPoint, Vector2 start, float angle, float length, float offset = 0)
        {
            var points = new List<Vector2>(2);
            AppendLineFromRef(points, refPoint, start, angle, length, offset);
            return points.ToArray();
        }

        public static Vector2[] ParallelLine(Vector2 start, Vector2 end, float distance, float startOffset = 0, float endOffset = 0)
        {
            var points = new List<Vector2>(2);
            AppendParallelLine(points, start, end, distance, startOffset, endOffset);
            return points.ToArray();
        }

        public static Vector2[] ParallelLines(Vector2 start, Vector2 end, float distance, int count)
        {
            var points = new List<Vector2>(2 * count);
            AppendParallelLines(points, start, end, distance, count);
            return points.ToArray();
        }

        public static Vector2[] ParallelLines(Vector2 refStart, Vector2 refEnd, IList<float> distances)
        {
            var points = new List<Vector2>(2 * distances.Count);
            AppendParallelLines(points, refStart, refEnd, distances);
            return points.ToArray();
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

        //todo move to charts
        public static Vector2[] SegmentedLine(Vector2 start, Vector2 end, int segmentCount)
        {
            var segmentLength = (end - start).Length() / segmentCount;
            var points = new List<Vector2>(2 + 2 * (segmentCount + 1));
            var distances = Enumerable.Range(0, segmentCount + 1).Select(i => Min(1, i) * segmentLength).ToArray();

            AppendSegmentedLine(points, start, start.DirectionTo(end), distances);
            return points.ToArray();
        }

        //todo move to charts
        public static Vector2[] SegmentedArrow(Vector2 start, Vector2 top, float segmentLength,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var segmentCount = (int)((top - start).Length() / segmentLength);

            return SegmentedArrow(start, start.DirectionTo(top),
                Enumerable.Repeat(segmentLength, segmentCount - 1).ToArray(),
                headRadius, arrowAngle);
        }

        //todo move to charts
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

        //todo move to charts
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
                yield return new Vector2(radius * Cos(angle) + center.X, radius * Sin(angle) + center.Y);
            }
        }

        public static Vector2[] RegularConvexPolygon(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var points = new List<Vector2>((verticesCount + 1) * 2);
            AppendRegularConvexPolygon(points, center, radius, verticesCount, rotationAngle);
            return points.ToArray();
        }

        //todo move to charts
        public static Vector2[] Candlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            var points = new List<Vector2>(6 * 2);
            AppendCandlestick(points, low, lowOffset, high, highOffset, halfWidth);
            return points.ToArray();
        }

        //todo move to charts
        public static Vector2[] CrossedCandlestick(Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            var points = new List<Vector2>(7 * 2);
            AppendCrossedCandlestick(points, low, lowOffset, high, highOffset, halfWidth, upDirrection);
            return points.ToArray();
        }

        #endregion static

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
            var dir = start.DirectionTo(end);
            var count = (int)((end - start).Length() / spaceLength);

            for (int i = 0; i <= count; i++)
                AppendDot(points, start + dir * i * (1 + spaceLength));
        }

        public static void AppendDottedLineAdapted(IList<Vector2> points, Vector2 start, Vector2 end,
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
            var count = (int)((end - start).Length() / (spaceLength + dashLength));
            var dir = start.DirectionTo(end);

            for (int i = 0; i < count; i++)
            {
                var segmentStart = start + dir * i * (dashLength + spaceLength);
                AppendLine(points, segmentStart, segmentStart + dir * dashLength);
            }
        }

        public static void AppendDashedLineAdapted(IList<Vector2> points, Vector2 start, Vector2 end,
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
            var count = (int)((end - start).Length() / (spaceLength + 1 + spaceLength + dashLength));
            var dir = start.DirectionTo(end);

            for (int i = 0; i < count - 1; i++)
            {
                var segmentStart = start + dir * i * (dashLength + spaceLength + 1 + spaceLength);
                AppendLine(points, segmentStart, segmentStart + dir * dashLength);
                AppendDot(points, segmentStart + dir * (dashLength + spaceLength));
            }

            AppendLine(points, start + dir * (count - 1) * (dashLength + spaceLength + 1 + spaceLength), end);
        }

        public static void AppendDashDottedLineAdapted(IList<Vector2> points, Vector2 start, Vector2 end,
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
            var dir = start.DirectionTo(end);

            points.Add(start + dir * startOffset);
            points.Add(end - dir * endOffset);
        }

        public static void AppendLine(IList<Vector2> points, Vector2 start, Vector2 end)
        {
            points.Add(start);
            points.Add(end);
        }

        public static void AppendLine(IList<Vector2> points, float startX, float startY, float endX, float endY)
            => AppendLine(points, new(startX, startY), new(endX, endY));

        private static void AppendLine(Vector2[] points, int index, Vector2 start, Vector2 end)
        {
            points[index] = start;
            points[index + 1] = end;
        }

        /// <summary>
        /// Append a continuation line from the last point.
        /// </summary>
        public static void AppendLine(IList<Vector2> points, Vector2 end)
        {
            var last = points[points.Count - 1];

            points.Add(last);
            points.Add(last + end);
        }

        /// <summary>
        /// Append a continuation line relative to the last line by angle and length.
        /// </summary>
        /// <param name="points"> Points appending to. </param>
        /// <param name="angle"> Relative angle to last line. </param>
        /// <param name="length"> Line length. </param>
        public static void AppendLineFromRef(IList<Vector2> points, float angle, float length)
        {
            var start = points[points.Count - 1];
            var refLineAngle = points[points.Count - 2].DirectionTo(start).Angle();

            points.Add(start);
            points.Add(start + (Vector2.Right * length).Rotated(refLineAngle + angle));
        }

        /// <summary>
        /// Append a continuation line by angle and length and offset relative to the reference points.
        /// </summary>
        public static void AppendLineFromRef(IList<Vector2> points, Vector2 refPoint, Vector2 start, float angle, float length, float offset = 0)
        {
            var angle2 = (start - refPoint).Angle() + angle;
            var start2 = start + (Vector2.Right * offset).Rotated(angle2);

            points.Add(start2);
            points.Add(start2 + (Vector2.Right * length).Rotated(angle2));
        }

        /// <summary> Append a continuation line parallel to reference line given by two points. </summary>
        public static void AppendParallelLine(IList<Vector2> points, Vector2 refStart, Vector2 refEnd, float distance, float startOffset = 0, float endOffset = 0)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            AppendLine(points,
                refStart + dir * startOffset + normal * distance,
                refEnd + dir * endOffset + normal * distance);
        }

        /// <summary> Append a continuation line parallel to line from given points. </summary>
        public static void AppendParallelLines(IList<Vector2> points, Vector2 refStart, Vector2 refEnd, float distance, int count)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            for (int i = 0; i < count; i++)
            {
                AppendLine(points,
                    refStart + normal * distance * i,
                    refEnd + normal * distance * i);
            }
        }

        public static void AppendParallelLines(IList<Vector2> points, Vector2 refStart, Vector2 refEnd, IList<float> distances)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                AppendLine(points,
                    refStart + normal * distSum,
                    refEnd + normal * distSum);
            }
        }

        /// <summary>
        /// Append parallel lines alongside normal line determined by two points.
        /// </summary>
        public static void AppendParallelLinesAlong(IList<Vector2> points, Vector2 refStart, Vector2 refEnd, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();
            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                AppendLine(points,
                    refStart + dir * distSum,
                    refEnd + dir * distSum);
            }
        }

        public static void AppendCross(IList<Vector2> points, Vector2 center, float radius)
        {
            AppendLine(points, center.X - radius, center.Y, center.X + radius, center.Y);
            AppendLine(points, center.X, center.Y - radius, center.X, center.Y + radius);
        }

        public static void AppendCross2(IList<Vector2> points, Vector2 center, float outerRadius, float innerRadius)
        {
            AppendLine(points, center.X - innerRadius, center.Y, center.X - outerRadius, center.Y);
            AppendLine(points, center.X + innerRadius, center.Y, center.X + outerRadius, center.Y);
            AppendLine(points, center.X, center.Y - innerRadius, center.X, center.Y - outerRadius);
            AppendLine(points, center.X, center.Y + innerRadius, center.X, center.Y + outerRadius);
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

        //todo move to charts
        public static void AppendSegmentedLine(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();

            AppendLine(points, start, start + dir * distances.Sum());
            AppendParallelLines(points, start, dir.Normal1() * 3, distances); //todo replace var by offset
        }

        //todo move to charts
        public static void AppendSegmentedArrow(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var dir = direction.Normalized();
            var segmentedPartEnd = start + dir * distances.Sum();

            AppendArrow(points, start, segmentedPartEnd + dir * 2 * headRadius, headRadius, arrowAngle);
            //todo replace var by offset
            AppendParallelLinesAlong(points, start, start + dir.Normal1() * 3, start.DirectionTo(segmentedPartEnd), distances);
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

        //todo move to charts
        public static void AppendAxes(IList<Vector2> points, Vector2 origin, Vector2 xDirection, float xUnitLength, int xUnitCount, float yUnitLength, int yUnitCount,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var xDistances = Enumerable.Range(0, xUnitCount).Select(i => xUnitLength).ToArray();
            var yDistances = Enumerable.Range(0, yUnitCount).Select(i => yUnitLength).ToArray();

            AppendSegmentedArrow(points, origin, xDirection, xDistances, headRadius, arrowAngle);
            AppendSegmentedArrow(points, origin, xDirection.Normal1(), yDistances, headRadius, arrowAngle);
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
            var dir = vertex1.DirectionTo(vertex2);
            var normal = dir.Normal2();
            var vertex3 = vertex2 + normal * height;
            var vertex4 = vertex1 + normal * height;

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

        //todo move to charts
        public static void AppendCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            var dir = low.DirectionTo(high);
            var rectBottom = low + dir * lowOffset;
            var rectTop = high - dir * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
        }

        //todo move to charts
        public static void AppendCrossedCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            var dir = low.DirectionTo(high);
            var rectBottom = low + dir * lowOffset;
            var rectTop = high - dir * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
            if (upDirrection)
            {
                AppendLine(points,
                    new Vector2(rectBottom.X - halfWidth, rectBottom.Y),
                    new Vector2(rectTop.X + halfWidth, rectTop.Y));
            }
            else
                AppendLine(points,
                    new Vector2(rectBottom.X + halfWidth, rectBottom.Y),
                    new Vector2(rectTop.X - halfWidth, rectTop.Y));
        }

        //todo move to charts
        private static void AppendCandlestick(IList<Vector2> points, Vector2 bottom, Vector2 top, Vector2 rectBottom, Vector2 rectCenter, Vector2 rectTop, float bodyHalfWidth)
        {
            AppendLine(points, bottom, rectBottom);
            AppendLine(points, top, rectTop);
            AppendRectangle(points, rectCenter, (rectCenter - rectBottom).Length(), bodyHalfWidth, bottom.DirectionTo(top).Angle());
        }

        #endregion static appending
    }
}