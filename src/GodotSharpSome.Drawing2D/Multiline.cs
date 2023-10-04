namespace GodotSharpSome.Drawing2D
{
    using System.Diagnostics;
    using System.Linq;
    using static Godot.Mathf;

    [DebuggerDisplay("palette:{_penPalette.Count}, points:{_points.Count}")]
    public class Multiline
    {
        //Default values
        private const float Arrow_HeadAngle = Pi / 14;

        private const float Arrow_HeadRadius = 20;

        private static readonly Vector2 DotVector = Vector2.Down;

        public static Multiline FourLineTypes() =>
            new(
                (nameof(LineType.Solid), new SolidLine()),
                (nameof(LineType.Dotted), new DottedLine()),
                (nameof(LineType.Dashed), new DashedLine()),
                (nameof(LineType.DashDotted), new DashDottedLine()));

        private readonly IList<Vector2> _points;

        private IList<(string Key, IStraightLineAppender Pen)>? _penPalette;

        /// <summary> Current pen. </summary>
        private IStraightLineAppender _pen;

        private string? _penKey;

        public Vector2[] Points() => _points.ToArray();

        public Multiline()
            : this(new List<Vector2>())
        { }

        public Multiline(int capacity)
            : this(new List<Vector2>(capacity))
        { }

        public Multiline(IList<Vector2> points)
            : this(points, new SolidLine(), nameof(LineType.Solid))
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
            _pen = palette[0].Pen;
            _penKey = palette[0].Key;
        }

        protected IList<(string Key, IStraightLineAppender Pen)> PenPalette =>
            _penPalette ??= new List<(string Key, IStraightLineAppender Pen)>();

        /// <summary>
        /// Current pen (line appender).
        /// </summary>
        public IStraightLineAppender Pen => _pen;

        public string? PenKey => _penKey;

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

        public Multiline AppendArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            Pen.AppendLine(_points, start, top);
            AppendArrowHead(start.DirectionTo(top), top, headRadius, arrowAngle);

            return this;
        }

        public Multiline AppendDoubleArrow(Vector2 start, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            Pen.AppendLine(_points, start, top);
            AppendArrowHead(start.DirectionTo(top), top, headRadius, arrowAngle);
            AppendArrowHead(top.DirectionTo(start), start, headRadius, arrowAngle);

            return this;
        }

        public Multiline AppendArrowHead(Vector2 direction, Vector2 top,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            //side line 1
            Pen.AppendLine(_points, top, top + direction.Rotated(Pi + arrowAngle) * headRadius);
            //side line 2
            Pen.AppendLine(_points, top, top + direction.Rotated(Pi - arrowAngle) * headRadius);

            return this;
        }

        public Multiline AppendVectorsRelatively(Vector2 zero, IEnumerable<Vector2> vectors,
            float arrowAngle = Arrow_HeadAngle)
        {
            var offset = zero;
            foreach (var vector in vectors)
                AppendArrow(offset, offset += vector, Clamp(vector.Length() / 4f, 14, 20), arrowAngle);

            return this;
        }

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

        public Multiline AppendTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            Pen.AppendLine(_points, a, b);
            Pen.AppendLine(_points, b, c);
            Pen.AppendLine(_points, c, a);

            return this;
        }

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
    }
}