namespace GodotSharpSome.Drawing2D
{
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using static Godot.Mathf;

    public static class CanvasItemExtension
    {
        /// <summary>
        /// Draw a single dot.
        /// </summary>
        public static void DrawDot(this CanvasItem canvas, Vector2 position, Color color,
            bool antialiased = false)
        {
            canvas.DrawMultiline(Multiline.Dot(position), color, 1, antialiased);
        }

        /// <summary>
        /// Draw series of dots.
        /// </summary>
        public static void DrawDots(this CanvasItem canvas, IEnumerable<Vector2> positions, Color color,
            bool antialiased = false)
        {
            canvas.DrawMultiline(Multiline.Dots(positions), color, 1, antialiased);
        }

        /// <summary>
        /// Draw a circumference of a circle.
        /// </summary>
        public static void DrawCircleLine(this CanvasItem canvas, Vector2 center, float radius, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawArc(center, radius, 0, Pi * 2, (int)radius * 16, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a circle or disc.
        /// </summary>
        public static void DrawCircleArea(this CanvasItem canvas, Vector2 center, float radius, Color areaColor)
        {
            canvas.DrawCircle(center, radius, areaColor);
        }

        /// <summary>
        /// Draw a circumference and plane region of a circle.
        /// </summary>
        public static void DrawCircle(this CanvasItem canvas, Vector2 center, float radius, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawCircleArea(center, radius, areaColor);
            canvas.DrawCircleLine(center, radius, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a triangle.
        /// </summary>
        public static void DrawTriangleLine(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Triangle(a, b, c),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a triangle.
        /// </summary>
        public static void DrawTriangleArea(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Triangle(a, b, c),
                new Color[] { areaColor });
        }

        /// <summary>
        /// Draw a perimeter and plane region of a triangle.
        /// </summary>
        public static void DrawTriangle(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawTriangleArea(a, b, c, areaColor);
            canvas.DrawTriangleLine(a, b, c, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleLine(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleLine(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(vertex1, vertex2, height),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a rectangle.
        /// </summary>
        public static void DrawRectangleArea(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                new Color[] { areaColor });
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleArea(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Rectangle(vertex1, vertex2, height),
                new Color[] { areaColor });
        }

        /// <summary>
        /// Draw a perimeter and plane region of a rectangle.
        /// </summary>
        public static void DrawRectangle(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRectangleArea(center, length, width, rotationAngle, areaColor);
            canvas.DrawRectangleLine(center, length, width, rotationAngle, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter and plane region of a rectangle.
        /// </summary>
        public static void DrawRectangle(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRectangleArea(vertex1, vertex2, height, areaColor);
            canvas.DrawRectangleLine(vertex1, vertex2, height, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a regular convex polygon.
        /// </summary>
        public static void DrawRegularConvexPolygonLine(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.RegularConvexPolygon(center, radius, verticesCount, rotationAngle),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a regular convex polygon.
        /// </summary>
        public static void DrawRegularConvexPolygonArea(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.RegularConvexPolygonVertices(center, radius, verticesCount, rotationAngle)
                    .ToArray(),
                new Color[] { areaColor });
        }

        /// <summary>
        /// Draw a perimeter and plane region of a regular convex polygon.
        /// </summary>
        public static void DrawRegularConvexPolygon(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRegularConvexPolygonArea(center, radius, verticesCount, rotationAngle, areaColor);
            canvas.DrawRegularConvexPolygonLine(center, radius, verticesCount, rotationAngle, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a candlestick shape.
        /// </summary>
        public static void DrawCandlestick(this CanvasItem canvas, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, Color lineColor, Color bodyColor,
            float lineWidth = 1, bool antialiased = false)
        {
            var vector = high - low;
            canvas.DrawRectangleArea(
                center: low + vector.Normalized() * (vector.Length() + lowOffset - highOffset) / 2f,
                length: vector.Length() - lowOffset - highOffset,
                width: halfWidth * 2,
                rotationAngle: vector.Angle(),
                areaColor: bodyColor);

            canvas.DrawMultiline(
                Multiline.Candlestick(low, lowOffset, high, highOffset, halfWidth),
                lineColor, lineWidth, antialiased);
        }
    }
}
