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
        public static CanvasItem DrawDot(this CanvasItem canvas, Vector2 position, Color color,
            bool antialiased = false)
        {
            canvas.DrawMultiline(Multiline.Dot(position), color, 1, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw series of dots.
        /// </summary>
        public static CanvasItem DrawDots(this CanvasItem canvas, IList<Vector2> positions, Color color,
            bool antialiased = false)
        {
            canvas.DrawMultiline(Multiline.Dots(positions), color, 1, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a circumference of a circle.
        /// </summary>
        public static CanvasItem DrawCircleOutline(this CanvasItem canvas, Vector2 center, float radius, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawArc(center, radius, 0, Pi * 2, (int)radius * 16, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a plane region of a circle or disc.
        /// </summary>
        public static CanvasItem DrawCircleRegion(this CanvasItem canvas, Vector2 center, float radius, Color areaColor)
        {
            canvas.DrawCircle(center, radius, areaColor);
            
            return canvas;
        }

        /// <summary>
        /// Draw a circumference and plane region of a circle.
        /// </summary>
        public static CanvasItem DrawCircle(this CanvasItem canvas, Vector2 center, float radius, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawCircleRegion(center, radius, areaColor);
            canvas.DrawCircleOutline(center, radius, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a circumference of an ellipse.
        /// </summary>
        public static CanvasItem DrawEllipseOutline(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color lineColor,
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
            
            return canvas;
        }

        /// <summary>
        /// Draw a plane region of an ellipse.
        /// </summary>
        public static CanvasItem DrawEllipseRegion(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color areaColor)
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
            
            return canvas;
        }

        /// <summary>
        /// Draw a circumference and plane region of an ellipse.
        /// </summary>
        public static CanvasItem DrawEllipse(this CanvasItem canvas, Vector2 center, float radiusA, float radiusB, float angle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawEllipseRegion(center, radiusA, radiusB, angle, areaColor);
            canvas.DrawEllipseOutline(center, radiusA, radiusB, angle, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter of a triangle.
        /// </summary>
        public static CanvasItem DrawTriangleOutline(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Triangle(a, b, c),
                lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a plane region of a triangle.
        /// </summary>
        public static CanvasItem DrawTriangleRegion(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Triangle(a, b, c),
                new Color[] { areaColor });
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter and plane region of a triangle.
        /// </summary>
        public static CanvasItem DrawTriangle(this CanvasItem canvas, Vector2 a, Vector2 b, Vector2 c, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawTriangleRegion(a, b, c, areaColor);
            canvas.DrawTriangleOutline(a, b, c, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangleOutline(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangleOutline(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.Rectangle(vertex1, vertex2, height),
                lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a plane region of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangleRegion(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Rectangle(center, length / 2, width / 2, rotationAngle),
                new Color[] { areaColor });
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangleRegion(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.Rectangle(vertex1, vertex2, height),
                new Color[] { areaColor });
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter and plane region of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangle(this CanvasItem canvas, Vector2 center, float length, float width, float rotationAngle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRectangleRegion(center, length, width, rotationAngle, areaColor);
            canvas.DrawRectangleOutline(center, length, width, rotationAngle, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter and plane region of a rectangle.
        /// </summary>
        public static CanvasItem DrawRectangle(this CanvasItem canvas, Vector2 vertex1, Vector2 vertex2, float height, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRectangleRegion(vertex1, vertex2, height, areaColor);
            canvas.DrawRectangleOutline(vertex1, vertex2, height, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter of a regular convex polygon.
        /// </summary>
        public static CanvasItem DrawRegularConvexPolygonOutline(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color lineColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawMultiline(
                Multiline.RegularConvexPolygon(center, radius, verticesCount, rotationAngle),
                lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a plane region of a regular convex polygon.
        /// </summary>
        public static CanvasItem DrawRegularConvexPolygonRegion(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color areaColor)
        {
            canvas.DrawPolygon(
                Multiline.RegularConvexPolygonVertices(center, radius, verticesCount, rotationAngle)
                    .ToArray(),
                new Color[] { areaColor });
            
            return canvas;
        }

        /// <summary>
        /// Draw a perimeter and plane region of a regular convex polygon.
        /// </summary>
        public static CanvasItem DrawRegularConvexPolygon(this CanvasItem canvas, Vector2 center, float radius, int verticesCount, float rotationAngle, Color lineColor, Color areaColor,
            float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawRegularConvexPolygonRegion(center, radius, verticesCount, rotationAngle, areaColor);
            canvas.DrawRegularConvexPolygonOutline(center, radius, verticesCount, rotationAngle, lineColor, lineWidth, antialiased);
            
            return canvas;
        }

        /// <summary>
        /// Draw a candlestick shape.
        /// </summary>
        public static CanvasItem DrawCandlestick(this CanvasItem canvas, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, Color lineColor, Color bodyColor,
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
            
            return canvas;
        }

        public static CanvasItem DrawString(this CanvasItem canvas, Font font, Vector2 position, string text, float angle, Color? color = null)
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

            return canvas;
        }

        public static CanvasItem DrawCenteredString(this CanvasItem canvas, Font font, Vector2 position, string text, float angle, Color? color = null)
        {
            var originTransform = canvas.GetCanvasTransform();
            var textSize = font.GetStringSize(text);

            Transform2D t = Transform2D.Identity;
            t.origin = position;
            t.x.x = t.y.y = Cos(angle);
            t.x.y = t.y.x = Sin(angle);
            t.y.x *= -1;

            canvas.DrawSetTransformMatrix(t);
            canvas.DrawString(font, new Vector2(-textSize.x / 2f, 0), text, color);
            canvas.DrawSetTransformMatrix(originTransform);

            return canvas;
        }
    }
}
