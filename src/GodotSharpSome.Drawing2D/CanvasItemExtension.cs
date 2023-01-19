namespace GodotSharpSome.Drawing2D
{
    using System;
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
        public static void DrawDots(this CanvasItem canvas, IList<Vector2> positions, Color color,
            bool antialiased = false)
        {
            canvas.DrawMultiline(Multiline.Dots(positions), color, 1, antialiased);
        }

        /// <summary>
        /// Draw a circumference of a circle.
        /// </summary>
        public static void DrawCircleOutline(this CanvasItem canvas, Vector2 center, float radius, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawArc(center, radius, 0, Pi * 2, (int)radius * 16, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a circle or disc.
        /// </summary>
        public static void DrawCircleRegion(this CanvasItem canvas, Vector2 center, float radius, Color areaColor)
        {
            canvas.DrawCircle(center, radius, areaColor);
        }

        /// <summary>
        /// Draw a circumference and plane region of a circle.
        /// </summary>
        public static void DrawCircle(this CanvasItem canvas, Vector2 center, float radius, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawCircleRegion(center, radius, areaColor);
            canvas.DrawCircleOutline(center, radius, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a circumference of an ellipse.
        /// </summary>
        public static void DrawEllipseOutline(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            var originTransform = canvas.GetCanvasTransform();

            Transform2D t = Transform2D.Identity;
            t.origin = center;
            t.x.x = t.y.y = Cos(angle);
            t.x.y = t.y.x = Sin(angle);
            t.y.x *= -1;
            t.y *= radiusB / radiusA;

            canvas.DrawSetTransformMatrix(t);
            canvas.DrawCircleOutline(Vector2.Zero, radiusA, lineColor, lineWidth, antialiased);
            canvas.DrawSetTransformMatrix(originTransform);
        }

        /// <summary>
        /// Draw a plane region of an ellipse.
        /// </summary>
        public static void DrawEllipseRegion(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color areaColor)
        {
            var originTransform = canvas.GetCanvasTransform();

            Transform2D t = Transform2D.Identity;
            t.origin = center;
            t.x.x = t.y.y = Cos(angle);
            t.x.y = t.y.x = Sin(angle);
            t.y.x *= -1;
            t.y *= radiusB / radiusA;

            canvas.DrawSetTransformMatrix(t);
            canvas.DrawCircleRegion(Vector2.Zero, radiusA, areaColor);
            canvas.DrawSetTransformMatrix(originTransform);
        }

        /// <summary>
        /// Draw a circumference and plane region of an ellipse.
        /// </summary>
        public static void DrawEllipse(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawEllipseRegion(center, radiusA, radiusB, angle, areaColor);
            canvas.DrawEllipseOutline(center, radiusA, radiusB, angle, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a triangle.
        /// </summary>
        public static void DrawTriangleOutline(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Triangle(a, b, c),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a triangle.
        /// </summary>
        public static void DrawTriangleRegion(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color areaColor)
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
            canvas.DrawTriangleRegion(a, b, c, areaColor);
            canvas.DrawTriangleOutline(a, b, c, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleOutline(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleOutline(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(vertex1, vertex2, height),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a rectangle.
        /// </summary>
        public static void DrawRectangleRegion(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                new Color[] { areaColor });
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static void DrawRectangleRegion(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color areaColor)
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
            canvas.DrawRectangleRegion(center, length, width, rotationAngle, areaColor);
            canvas.DrawRectangleOutline(center, length, width, rotationAngle, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter and plane region of a rectangle.
        /// </summary>
        public static void DrawRectangle(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRectangleRegion(vertex1, vertex2, height, areaColor);
            canvas.DrawRectangleOutline(vertex1, vertex2, height, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a perimeter of a regular convex polygon.
        /// </summary>
        public static void DrawRegularConvexPolygonOutline(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.RegularConvexPolygon(center, radius, verticesCount, rotationAngle),
                lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a plane region of a regular convex polygon.
        /// </summary>
        public static void DrawRegularConvexPolygonRegion(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color areaColor)
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
            canvas.DrawRegularConvexPolygonRegion(center, radius, verticesCount, rotationAngle, areaColor);
            canvas.DrawRegularConvexPolygonOutline(center, radius, verticesCount, rotationAngle, lineColor, lineWidth, antialiased);
        }

        /// <summary>
        /// Draw a candlestick shape.
        /// </summary>
        public static void DrawCandlestick(this CanvasItem canvas, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, Color lineColor, Color bodyColor,
            float lineWidth = 1, bool antialiased = false)
        {
            var vector = high - low;
            canvas.DrawRectangleRegion(
                center: low + vector.Normalized() * (vector.Length() + lowOffset - highOffset) / 2f,
                length: vector.Length() - lowOffset - highOffset,
                width: halfWidth * 2,
                rotationAngle: vector.Angle(),
                areaColor: bodyColor);

            canvas.DrawMultiline(
                Multiline.Candlestick(low, lowOffset, high, highOffset, halfWidth),
                lineColor, lineWidth, antialiased);
        }

        public static void DrawString(this CanvasItem canvas, Font font, Vector2 position, string text, float angle, Color? color = null)
        {
            var originTransform = canvas.GetCanvasTransform();

            Transform2D t = Transform2D.Identity;
            t.origin = position;
            t.x.x = t.y.y = Cos(angle);
            t.x.y = t.y.x = Sin(angle);
            t.y.x *= -1;

            canvas.DrawSetTransformMatrix(t);
            canvas.DrawString(font, Vector2.Zero, text, color);
            canvas.DrawSetTransformMatrix(originTransform);
        }
    }
}
