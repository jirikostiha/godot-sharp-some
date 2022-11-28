namespace GodotSharpSome.Drawing2D.Tables
{
    using System.Collections.Generic;
    using Godot;

    public static class CanvasItemExtension //likely control ext
    {
        public static void DrawTable(this CanvasItem canvas, Vector2 topLeft, IList<string> header, IList<IList<string>> data, Font font, Color lineColor)
        {
            var points = new List<Vector2>((6 + data.Count + data[0].Count) * 2);

            var rowWidth = 200;
            var cellWidth = rowWidth / header.Count;
            var topRight = topLeft + Vector2.Right * rowWidth;
            var rowHeight = 20;
            var bodyTopLeft = topLeft + Vector2.Down * (rowHeight + 2);
            var bodyTopRight = bodyTopLeft + Vector2.Right * rowWidth;
            var tableHeight = rowHeight * (data.Count + 1) + 2;

            // header
            Multiline.AppendRectangle(points, topLeft, topRight, -rowHeight);

            // body
            for (int row = 0; row <= data.Count; row++)
            {
                var left = bodyTopLeft + row * rowHeight * Vector2.Down;
                Multiline.AppendLine(points, left, left + rowWidth * Vector2.Right);
            }

            for (int col = 0; col < header.Count; col++)
            {
                var top = topLeft + col * cellWidth * Vector2.Right;
                Multiline.AppendLine(points, top, top + tableHeight * Vector2.Down);
            }
            Multiline.AppendLine(points, topRight, topRight + tableHeight * Vector2.Down);

            canvas.DrawMultiline(points.ToArray(), lineColor);
        }
    }
}
