namespace GodotSharpSome.Drawing2D
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using static Godot.Mathf;

    /// <summary>
    /// Multiline builder.
    /// </summary>
    [DebuggerDisplay("palette:{_penPalette.Count}, segments:{Segments}, penkey:{PenKey}")]
    public class Multiline
    {
        /// <summary> Default angle value for arrow head. </summary>
        private const float Arrow_HeadAngle = Pi / 14;

        /// <summary> Default length of an arrow head. </summary>
        private const float Arrow_HeadRadius = 20;

        private static readonly Vector2 DotVector = Vector2.Down;

        /// <summary>
        /// Create multiline instance with palette of four basic line types.
        /// </summary>
        public static Multiline FourLineTypes() =>
            new(
                (nameof(LineType.Solid), SolidLine.Default),
                (nameof(LineType.Dotted), DottedLine.Default),
                (nameof(LineType.Dashed), DashedLine.Default),
                (nameof(LineType.DashDotted), DashDottedLine.Default));

        /// <summary>
        /// Create multiline instance with palette of four basic line types with custom parameters.
        /// </summary>
        public static Multiline FourLineTypes(float spaceLength, float dashLength) =>
            new(
                (nameof(LineType.Solid), SolidLine.Default),
                (nameof(LineType.Dotted), new DottedLine(spaceLength)),
                (nameof(LineType.Dashed), new DashedLine(spaceLength, dashLength)),
                (nameof(LineType.DashDotted), new DashDottedLine(spaceLength, dashLength)));

        /// <summary>
        /// Create multiline instance with palette of four basic line types with custom appenders.
        /// </summary>
        public static Multiline FourLineTypes(SolidLine solid, DottedLine dotted, DashedLine dashed, DashDottedLine dashDotted) =>
            new(
                (nameof(LineType.Solid), solid),
                (nameof(LineType.Dotted), dotted),
                (nameof(LineType.Dashed), dashed),
                (nameof(LineType.DashDotted), dashDotted));


        private readonly IList<Vector2> _points;

        private IList<(string Key, IStraightLineAppender Pen)>? _penPalette;

        /// <summary> Current pen. </summary>
        private IStraightLineAppender _pen;

        private string? _penKey;

        public Multiline()
            : this(new List<Vector2>())
        { }

        public Multiline(int capacity)
            : this(new List<Vector2>(capacity))
        { }

        public Multiline(IList<Vector2> points)
            : this(points, SolidLine.Default, nameof(LineType.Solid))
        { }

        public Multiline(IStraightLineAppender pen, string? penKey = default)
            : this(new List<Vector2>(), pen, penKey)
        { }

        public Multiline(int capacity, IStraightLineAppender pen, string? penKey = default)
            : this(new List<Vector2>(capacity), pen, penKey)
        { }

        public Multiline(IList<(string Key, IStraightLineAppender Pen)> palette)
            : this(new List<Vector2>(), palette)
        { }

        public Multiline(int capacity, IList<(string Key, IStraightLineAppender Pen)> palette)
            : this(new List<Vector2>(capacity), palette)
        { }

        public Multiline(params (string, IStraightLineAppender)[] palette)
            : this(new List<Vector2>(), (IList<(string, IStraightLineAppender)>)palette)
        { }

        public Multiline(int capacity, params (string, IStraightLineAppender)[] palette)
            : this(new List<Vector2>(capacity), (IList<(string, IStraightLineAppender)>)palette)
        { }

        public Multiline(IList<Vector2> points, params (string, IStraightLineAppender)[] palette)
            : this(points, (IList<(string, IStraightLineAppender)>)palette)
        { }

        public Multiline(IList<Vector2> points, IStraightLineAppender pen, string? penKey = default)
        {
            _points = points;
            _pen = pen;
            _penKey = penKey;
        }

        public Multiline(IList<Vector2> points, IList<(string Key, IStraightLineAppender Pen)> palette)
        {
            _points = points ?? new List<Vector2>();
            _penPalette = palette;
            Debug.Assert(palette.Count > 0, "Palette must have at least one item.");
            _pen = palette[0].Pen;
            _penKey = palette[0].Key;
        }

        protected IList<(string Key, IStraightLineAppender Pen)> PenPalette =>
            _penPalette ??= new List<(string Key, IStraightLineAppender Pen)>(4);

        /// <summary>
        /// Current pen (line appender).
        /// </summary>
        public IStraightLineAppender Pen => _pen;

        /// <summary>
        /// Current pen (line appender) key.
        /// </summary>
        public string? PenKey => _penKey;

        /// <summary>
        /// Number of line segments.
        /// Note: For example, a dashed line is a single line consisting of n segments.
        /// </summary>
        public int Segments => _points.Count / 2;

        public Vector2[] Points() => _points.ToArray();

        /// <summary>
        /// Set custom line type.
        /// </summary>
        public Multiline SetPen(IStraightLineAppender pen, string? penKey = null)
        {
            _pen = pen;
            _penKey = penKey;

            return this;
        }

        /// <summary>
        /// Set line type by key in palette.
        /// </summary>
        public Multiline SetPen(string key)
        {
            var item = PenPalette.First(x => x.Key == key);
            _pen = item.Pen;
            _penKey = item.Key;

            return this;
        }

        /// <summary>
        /// Set line type by index in palette.
        /// </summary>
        public Multiline SetPen(int index)
        {
            _pen = PenPalette[index].Pen;
            _penKey = PenPalette[index].Key;

            return this;
        }

        /// <summary>
        /// Append other multiline.
        /// </summary>
        public Multiline Append(Multiline other)
        {
            Debug.Assert(other._points.Count % 2 == 0, "Multiline has odd number of points. It must have even.");

            for (int i = 0; i < other._points.Count; i++)
                _points.Add(other._points[i]);

            return this;
        }

        /// <summary>
        /// Append single dot to position vector.
        /// </summary>
        public Multiline AppendDot(Vector2 position)
        {
            Pen.AppendLine(_points, position, position + DotVector);

            return this;
        }

        /// <summary>
        /// Append multiple dots to given positions.
        /// </summary>
        public Multiline AppendDots(IEnumerable<Vector2> positions)
        {
            foreach (var position in positions)
                AppendDot(position);

            return this;
        }

        /// <summary>
        /// Append straight line between two points.
        /// </summary>
        public Multiline AppendLine(Vector2 start, Vector2 end)
        {
            Pen.AppendLine(_points, start, end);

            return this;
        }

        /// <summary> Append a straight line from the last point. </summary>
        public Multiline AppendLine(Vector2 end)
        {
            var last = _points[_points.Count - 1];
            Pen.AppendLine(_points, last, end);

            return this;
        }

        /// <summary>
        /// Append straight line between two points with offset on both ends.
        /// </summary>
        public void AppendLine(Vector2 start, float startOffset, Vector2 end, float endOffset)
        {
            var dir = start.DirectionTo(end);
            Pen.AppendLine(_points, start + dir * startOffset, end - dir * endOffset);
        }

        /// <summary>
        /// Append a continuation line relative to the last line by angle and length.
        /// </summary>
        /// <param name="angle"> Relative angle to last line. </param>
        /// <param name="length"> Line length. </param>
        public Multiline AppendLineFromRef(float angle, float length)
        {
            var start = _points[_points.Count - 1];
            var refLineAngle = _points[_points.Count - 2].DirectionTo(start).Angle();
            Pen.AppendLine(_points, start, start + (Vector2.Right * length).Rotated(refLineAngle + angle));

            return this;
        }

        /// <summary>
        /// Append a continuation line by angle and length and offset relative to the reference points.
        /// </summary>
        public Multiline AppendLineFromRef(Vector2 refPoint, Vector2 start, float angle, float length, float offset = 0)
        {
            var angle2 = (start - refPoint).Angle() + angle;
            var start2 = start + (Vector2.Right * offset).Rotated(angle2);
            Pen.AppendLine(_points, start2, start2 + (Vector2.Right * length).Rotated(angle2));

            return this;
        }

        /// <summary> Append a continuation line parallel to reference line given by two points. </summary>
        public Multiline AppendParallelLine(Vector2 refStart, Vector2 refEnd, float distance, float startOffset = 0, float endOffset = 0)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            Pen.AppendLine(_points,
                refStart + dir * startOffset + normal * distance,
                refEnd + dir * endOffset + normal * distance);

            return this;
        }

        /// <summary> Append a continuation line parallel to line from given points. </summary>
        public Multiline AppendParallelLines(Vector2 refStart, Vector2 refEnd, float distance, int count)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            for (int i = 0; i < count; i++)
            {
                Pen.AppendLine(_points,
                    refStart + normal * distance * i,
                    refEnd + normal * distance * i);
            }

            return this;
        }

        /// <summary>
        /// Append parallel lines in given distances from reference line determined by two points.
        /// </summary>
        public Multiline AppendParallelLines(Vector2 refStart, Vector2 refEnd, IList<float> distances)
        {
            var dir = refStart.DirectionTo(refEnd);
            var normal = dir.Normal1();

            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                Pen.AppendLine(_points,
                    refStart + normal * distSum,
                    refEnd + normal * distSum);
            }

            return this;
        }

        /// <summary>
        /// Append parallel lines alongside normal line determined by two points.
        /// </summary>
        public Multiline AppendParallelLinesAlong(Vector2 refStart, Vector2 refEnd, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();
            var distSum = 0f;
            foreach (var distance in distances)
            {
                distSum += distance;
                Pen.AppendLine(_points,
                    refStart + dir * distSum,
                    refEnd + dir * distSum);
            }

            return this;
        }

        /// <summary>
        /// Append cross made by two orthogonal lines.
        /// </summary>
        public Multiline AppendCross(Vector2 center, float radius)
        {
            Pen.AppendLine(_points,
                new(center.X - radius, center.Y),
                new(center.X + radius, center.Y));
            Pen.AppendLine(_points,
                new(center.X, center.Y - radius),
                new(center.X, center.Y + radius));

            return this;
        }

        /// <summary>
        /// Append cross made by two orthogonal lines with empty center.
        /// </summary>
        public Multiline AppendCross2(Vector2 center, float outerRadius, float innerRadius)
        {
            Pen.AppendLine(_points,
                new(center.X - innerRadius, center.Y),
                new(center.X - outerRadius, center.Y));
            Pen.AppendLine(_points,
                new(center.X + innerRadius, center.Y),
                new(center.X + outerRadius, center.Y));
            Pen.AppendLine(_points,
                new(center.X, center.Y - innerRadius),
                new(center.X, center.Y - outerRadius));
            Pen.AppendLine(_points,
                new(center.X, center.Y + innerRadius),
                new(center.X, center.Y + outerRadius));

            return this;
        }

        /// <summary>
        /// Append an arrow from start point to end point with head in endpoint.
        /// </summary>
        public Multiline AppendArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            Pen.AppendLine(_points, start, top);
            AppendArrowHead(start.DirectionTo(top), top, headRadius, arrowAngle);

            return this;
        }

        /// <summary>
        /// Append arrow heads on both sides of line between two points.
        /// </summary>
        public Multiline AppendDoubleArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            Pen.AppendLine(_points, start, top);
            AppendArrowHead(start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(top.DirectionTo(start), start, headRadius, arrowAngle);

            return this;
        }

        /// <summary>
        /// Append an arrow head from determined point and arrow direction.
        /// </summary>
        public Multiline AppendArrowHead(Vector2 direction, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            //side line 1
            Pen.AppendLine(_points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            Pen.AppendLine(_points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);

            return this;
        }

        /// <summary>
        /// Append multiple arrows as vectors summation from determined point.
        /// </summary>
        public Multiline AppendVectorsRelatively(Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            var offset = zero;
            foreach (var vector in vectors)
                AppendArrow(offset, offset += vector, Clamp(vector.Length() / 4f, 14, 20), arrowAngle);

            return this;
        }

        /// <summary>
        /// Append arrows as vectors from determined point.
        /// </summary>
        public Multiline AppendVectorsAbsolutely(Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            foreach (var vector in vectors)
                AppendArrow(zero, zero + vector, Clamp(vector.Length() / 4f, 14, 20), arrowAngle);

            return this;
        }

        /// <summary>
        /// Append rectangle by center, half sizes and orientation.
        /// </summary>
        /// <param name="center"> Rectangle center. </param>
        /// <param name="halfLength"> Half size of rectangle length. </param>
        /// <param name="halfWidth"> Half size of rectangle width. </param>
        /// <param name="rotationAngle"> Orientation in radians. </param>
        public Multiline AppendRectangle(Vector2 center, float halfLength, float halfWidth, float rotationAngle)
        {
            var vertex1 = center + new Vector2(-halfLength, -halfWidth).Rotated(rotationAngle);
            var vertex2 = center + new Vector2(-halfLength, halfWidth).Rotated(rotationAngle);

            AppendRectangle(vertex1, vertex2, 2 * halfLength);

            return this;
        }

        /// <summary>
        /// Append rectangle by two vertices and height.
        /// </summary>
        /// <param name="vertex1"> Primary vertex. </param>
        /// <param name="vertex2"> Secondary vertex setting up base side of the rectangle. </param>
        /// <param name="height"> Distance of other side from base side. Positive is on left side of direction vertex/vector. </param>
        public Multiline AppendRectangle(Vector2 vertex1, Vector2 vertex2, float height)
        {
            var dir = vertex1.DirectionTo(vertex2);
            var normal = dir.Normal2();
            var vertex3 = vertex2 + normal * height;
            var vertex4 = vertex1 + normal * height;

            Pen.AppendLine(_points, vertex1, vertex2);
            Pen.AppendLine(_points, vertex2, vertex3);
            Pen.AppendLine(_points, vertex3, vertex4);
            Pen.AppendLine(_points, vertex4, vertex1);

            return this;
        }

        /// <summary>
        /// Append triangle determined by three points.
        /// </summary>
        public Multiline AppendTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            Pen.AppendLine(_points, a, b);
            Pen.AppendLine(_points, b, c);
            Pen.AppendLine(_points, c, a);

            return this;
        }

        /// <summary>
        /// Append regular convex polygon determined by center point, radius, number of vertices and initial rotation towards center.
        /// </summary>
        public Multiline AppendRegularConvexPolygon(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var vertices = RegularConvexPolygonVertices(center, radius, verticesCount, rotationAngle)
                .ToArray();

            for (int i = 1; i < vertices.Length; i++)
                Pen.AppendLine(_points, vertices[i - 1], vertices[i]);

            // closing line
            Pen.AppendLine(_points, vertices[verticesCount - 1], vertices[0]);

            return this;
        }

        /// <summary>
        /// Mirror all points to line parallel to x-axis.
        /// </summary>
        /// <param name="y"> Y coordinate of a mirror line. </param>
        public Multiline MirrorX(float y)
        {
            var count = _points.Count;
            for (int i = 0; i < count; i++)
                _points.Add(_points[i].MirrorX(y));

            return this;
        }

        /// <summary>
        /// Mirror all points to line parallel to y-axis.
        /// </summary>
        /// <param name="x"> X coordinate of a mirror line. </param>
        public Multiline MirrorY(float x)
        {
            var count = _points.Count;
            for (int i = 0; i < count; i++)
                _points.Add(_points[i].MirrorY(x));

            return this;
        }

        /// <summary>
        /// Mirror all points to line determined by one point and direction vector.
        /// </summary>
        /// <param name="mirrorPoint"> Point of mirror line. </param>
        /// <param name="mirrorLineDir"> Direction vector of mirror line. </param>
        public Multiline MirrorByDirection(Vector2 mirrorPoint, Vector2 mirrorLineDir)
        {
            var count = _points.Count;
            for (int i = 0; i < count; i++)
                _points.Add(_points[i].MirrorByDirection(mirrorPoint, mirrorLineDir));

            return this;
        }

        /// <summary>
        /// Mirror all points to line determined by one point and direction vector.
        /// </summary>
        /// <param name="mirrorPointA"> First point of mirror line. </param>
        /// <param name="mirrorPointB"> Second point of mirror line. </param>
        public Multiline MirrorByPoints(Vector2 mirrorPointA, Vector2 mirrorPointB)
        {
            var count = _points.Count;
            for (int i = 0; i < count; i++)
                _points.Add(_points[i].MirrorByPoints(mirrorPointA, mirrorPointB));

            return this;
        }

        /// <summary>
        /// Rotate all points around given center.
        /// </summary>
        /// <param name="center"> Rotation center. </param>
        /// <param name="angle"> Rotation angle. </param>
        public Multiline Rotate(Vector2 center, float angle)
        {
            var count = _points.Count;
            for (int i = 0; i < count; i++)
                _points.Add(_points[i].Rotated(center, angle));

            return this;
        }

        /// <summary>
        /// Rotate all points around given center and angles.
        /// </summary>
        /// <param name="center"> Rotation center. </param>
        /// <param name="angle"> Rotation angle. </param>
        /// <param name="repeatCount"> Number of a rotation repetition. </param>
        public Multiline Rotate(Vector2 center, float angle, int repeatCount)
        {
            var count = _points.Count;
            var currentAngle = 0f;
            for (int j = 0; j < repeatCount; j++)
            {
                currentAngle += angle;
                for (int i = 0; i < count; i++)
                    _points.Add(_points[i].Rotated(center, currentAngle));
            }

            return this;
        }

        /// <summary>
        /// Rotate all points around given center and angles.
        /// </summary>
        /// <param name="center"> Rotation center. </param>
        /// <param name="angles"> Rotation angles. </param>
        public Multiline Rotate(Vector2 center, params float[] angles)
        {
            var count = _points.Count;
            for (int angle = 0; angle < angles.Length; angle++)
            {
                for (int i = 0; i < count; i++)
                    _points.Add(_points[i].Rotated(center, angle));
            }

            return this;
        }

        /// <summary>
        /// Remove last line (two points) from collection of points.
        /// </summary>
        /// <returns></returns>
        public Multiline RemoveLast()
        {
            if (_points.Count > 1)
            {
                _points.RemoveAt(_points.Count - 1);
                _points.RemoveAt(_points.Count - 1);
            }
            return this;
        }

        /// <summary>
        /// Remove all points from collection.
        /// </summary>
        public Multiline Clear()
        {
            _points.Clear();
            return this;
        }

        /// <summary>
        /// Get position of vertices of regular convex polygon.
        /// </summary>
        public static IEnumerable<Vector2> RegularConvexPolygonVertices(Vector2 center, float radius, int verticesCount, float rotationAngle)
        {
            var segmentAngle = 2 * Pi / verticesCount;
            float angle;
            for (int i = 0; i < verticesCount; i++)
            {
                angle = rotationAngle + segmentAngle * i;
                yield return new(radius * Cos(angle) + center.X, radius * Sin(angle) + center.Y);
            }
        }

        public static Multiline operator +(Multiline a, Multiline b) => a.Append(b);
    }
}