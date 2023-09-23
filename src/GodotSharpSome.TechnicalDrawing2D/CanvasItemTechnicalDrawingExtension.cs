namespace GodotSharpSome.Drawing2D
{
    using System;
    using Godot;

    //https://en.wikipedia.org/wiki/Technical_drawing#Assembly_drawings
    //kóta — dimension
    public static class CanvasItemTechnicalDrawingExtension
    {
        public static void DrawInnerDimmension(this CanvasItem canvas, Vector2 point1, Vector2 point2, Color lineColor, string text, Color textColor, Font? font = null,
            float dimDistance = 14, float lineWidth = 1)
        {
            canvas.DrawMultiline(
                new Multiline().AppendDoubleArrow(point1, point2, dimDistance).Points(),
                lineColor, lineWidth);

            var dirVector = point1.DirectionTo(point2);
            var normalVector = new Vector2(dirVector.Y, -dirVector.X);
            var textCenter = point1 + ((point2 - point1) / 2f) + normalVector * (dimDistance + 4);
            canvas.DrawString(font, textCenter, text, 0, textColor);
        }

        public static void DrawOuterDimmension(this CanvasItem canvas, Vector2 point1, Vector2 point2, Color lineColor, string text, Color textColor, Font? font = null,
            float dimDistance = 14, float lineWidth = 1)
        {
            canvas.DrawMultiline(
                new Multiline().AppendDoubleArrow(point1, point2, dimDistance, 20).Points(),
                lineColor, lineWidth);

            var dirVector = point1.DirectionTo(point2);
            var normalVector = new Vector2(dirVector.Y, -dirVector.X);
            var textCenter = point1 + ((point2 - point1) / 2f) + normalVector * (dimDistance + 4);
            canvas.DrawString(font, textCenter, text, 0, textColor);
        }

        /*
                 private static void AppendInnerDimmensionLength(IList<Vector2> points, Vector2 point1, Vector2 point2,
            float dimDistance = Default_Dimmension_Distance, float headRadius = Default_DimmensionArrow_HeadRadius, float arrowAngle = Default_DimmensionArrow_HeadAngle)
        {
            var dirVector = point1.DirectionTo(point2);
            var normalVector = new Vector2(dirVector.y, -dirVector.x);

            AppendLine(points, point1, point1 + normalVector * (dimDistance + 4));
            AppendLine(points, point2, point2 + normalVector * (dimDistance + 4));
            AppendDoubleArrow(points, point1 + normalVector * dimDistance, point2 + normalVector * dimDistance, headRadius, arrowAngle);
        }

        //private static void AppendOuterDimmensionLength(IList<Vector2> points, Vector2 point1, Vector2 point2, float dimDistance,
        //    float headRadius = Default_Arrow_HeadRadius, float arrowAngle = Default_Arrow_HeadAngle)
        //{
        //    var dirVector = point1.DirectionTo(point2);
        //    var normalVector = new Vector2(dirVector.y, -dirVector.x);

        //    AppendLine(points, point1, point1 + normalVector * (dimDistance + 4));
        //    AppendLine(points, point2, point2 + normalVector * (dimDistance + 4));
        //    AppendDoubleArrow(points, point1 + normalVector * dimDistance, point2 + normalVector * dimDistance, headRadius, arrowAngle);
        //}

         */
    }
}
