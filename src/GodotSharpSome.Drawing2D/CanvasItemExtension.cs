namespace GodotSharpSome.Drawing2D
{
    using System.Collections;
    using System.Collections.Generic;
    using Godot;
    using static Godot.Mathf;

    public static class CanvasItemExtension
    {
        public static void DrawCircleLine(this CanvasItem canvas, Vector2 center, float radius, Color lineColor, float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawArc(center, radius, 0, Pi * 2, (int)radius * 16, lineColor, lineWidth, antialiased);
        }

        public static void DrawCircleArea(this CanvasItem canvas, Vector2 center, float radius, Color areaColor)
        {
            canvas.DrawCircle(center, radius, areaColor);
        }

        public static void DrawCircle(this CanvasItem canvas, Vector2 center, float radius, Color lineColor, Color areaColor, float lineWidth = 1, bool antialiased = false)
        {
            canvas.DrawCircleArea(center, radius, areaColor);
            canvas.DrawCircleLine(center, radius, lineColor, lineWidth, antialiased);
        }
    }
}
