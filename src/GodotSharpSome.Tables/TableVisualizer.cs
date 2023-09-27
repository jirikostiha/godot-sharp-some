namespace GodotSharpSome.Tables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using GodotSharpSome.Grids2D;

    public class TableVisualizer
    {
        private OrthogonalGridVisualizer _gridVizer;

        public TableVisualizer(Control canvas)
        {
            Canvas = canvas;
            _gridVizer = new OrthogonalGridVisualizer(canvas);
        }

        protected CanvasItem Canvas { get; set; }

        public TableVOptions VOptions { get; set; } = new();

        public Vector2 Position { get; set; }

        public void Draw(IList<(string C1, string C2)> rows)
        {
            _gridVizer.Draw(new () { XCount = 2, YCount = rows.Count });
        }

        public void Draw(IList<string[]> rows)
        {
            var margin = VOptions.TextMargin;
            var font = VOptions.BodyFont;
            var textHeight = font.GetStringSize("A").Y;
            var rowHeight = textHeight + 2 * VOptions.TextMargin;
            var maxTextWidth = 50;
            var cellWidth = maxTextWidth + 2 * margin;
            //var maxTextWidths = (
            //    rows.Max(r => font.GetStringSize(r.C1).X),
            //    rows.Max(r => font.GetStringSize(r.C2).X));
            //var cellWidths = (maxTextWidths.Item1 + 2 * margin, maxTextWidths.Item2 + 2 * margin);

            _gridVizer.Draw(null);

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    Canvas.DrawString(font,
                        new Vector2(Position.X + margin + cellWidth, Position.Y + margin + textHeight + i * rowHeight),
                        rows[i][j], HorizontalAlignment.Left, -1, 16, Color.FromHtml("gray"));
                }
            }
        }

        public void DrawTable(IList<(string C1, string C2)> rows)
        {
            var margin = VOptions.TextMargin;
            var font = VOptions.BodyFont;
            var textHeight = font.GetStringSize("A").Y;
            var rowHeight = textHeight + 2 * margin;
            var maxTextWidth = 50;
            var cellWidth = maxTextWidth + 2 * margin;

            var maxTextWidths = (
                rows.Max(r => font.GetStringSize(r.C1).X),
                rows.Max(r => font.GetStringSize(r.C2).X));
            var cellWidths = (maxTextWidths.Item1 + 2 * margin, maxTextWidths.Item2 + 2 * margin);

            var mb = new Multiline(2 * (rows.Count + 1) + 2 * 3);

            AppendTableLines(mb, rowHeight, rows.Count, new[] { 0f, cellWidths.Item1, cellWidths.Item2 }, origin);

            Canvas.DrawMultiline(mb.Points(), foreColor);

            for (int i = 0; i < rows.Count; i++)
            {
                Canvas.DrawString(font,
                    new Vector2(origin.X + margin, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C1, 0, foreColor);

                Canvas.DrawString(font,
                    new Vector2(origin.X + margin + cellWidths.Item1, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C2, 0, foreColor);
            }
        }

        public void DrawTable(IList<(string C1, string C2, string C3)> rows, Font font, Color foreColor, Vector2 origin = default)
        {
            var margin = VOptions.TextMargin;
            var textHeight = font.GetStringSize("A").Y;
            var rowHeight = textHeight + 2 * margin;
            var maxTextWidths = (
                rows.Max(r => font.GetStringSize(r.C1).X),
                rows.Max(r => font.GetStringSize(r.C2).X),
                rows.Max(r => font.GetStringSize(r.C3).X));
            var cellWidths = (maxTextWidths.Item1 + 2 * margin, maxTextWidths.Item2 + 2 * margin, maxTextWidths.Item3 + 2 * margin);

            var mb = new Multiline(2 * (rows.Count + 1) + 2 * 4);

            AppendTableLines(mb, rowHeight, rows.Count, new[] { 0f, cellWidths.Item1, cellWidths.Item2, cellWidths.Item3 }, origin);

            Canvas.DrawMultiline(mb.Points(), foreColor);

            for (int i = 0; i < rows.Count; i++)
            {
                Canvas.DrawString(font,
                    new Vector2(origin.X + margin, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C1, 0, foreColor);

                Canvas.DrawString(font,
                    new Vector2(origin.X + margin + cellWidths.Item1, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C2, 0, foreColor);

                Canvas.DrawString(font,
                    new Vector2(origin.X + margin + cellWidths.Item1 + cellWidths.Item2, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C3, 0, foreColor);
            }
        }

        public void DrawTable(IList<(string C1, string C2, string C3, string C4)> rows, Font font, Color foreColor, Vector2 origin = default)
        {
            var margin = VOptions.TextMargin;
            var textHeight = font.GetStringSize("A").Y;
            var rowHeight = textHeight + 2 * margin;
            var maxTextWidths = (
                rows.Max(r => font.GetStringSize(r.C1).X),
                rows.Max(r => font.GetStringSize(r.C2).X),
                rows.Max(r => font.GetStringSize(r.C3).X),
                rows.Max(r => font.GetStringSize(r.C4).X));
            var cellWidths = (maxTextWidths.Item1 + 2 * margin, maxTextWidths.Item2 + 2 * margin, maxTextWidths.Item3 + 2 * margin, maxTextWidths.Item4 + 2 * margin);

            var mb = new Multiline(2 * (rows.Count + 1) + 2 * 4);

            AppendTableLines(mb, rowHeight, rows.Count, new[] { 0f, cellWidths.Item1, cellWidths.Item2, cellWidths.Item3, cellWidths.Item4 }, origin);

            Canvas.DrawMultiline(mb.Points(), foreColor);

            for (int i = 0; i < rows.Count; i++)
            {
                Canvas.DrawString(font,
                    new Vector2(origin.X + margin, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C1, 0, foreColor);

                Canvas.DrawString(font,
                    new Vector2(origin.X + margin + cellWidths.Item1, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C2, 0, foreColor);

                Canvas.DrawString(font,
                    new Vector2(origin.X + margin + cellWidths.Item1 + cellWidths.Item2, origin.Y + margin + textHeight + i * rowHeight),
                    rows[i].C3, 0, foreColor);

                Canvas.DrawString(font,
                   new Vector2(origin.X + margin + cellWidths.Item1 + cellWidths.Item2 + cellWidths.Item3, origin.Y + margin + textHeight + i * rowHeight),
                   rows[i].C4, 0, foreColor);
            }
        }

        protected static Multiline AppendTableLines(Multiline mb, float rowHeight, int rowCount, IList<float> cellWidths, Vector2 origin = default)
        {
            // horizontal lines
            mb.AppendParallelLines(
                origin,
                new Vector2(origin.X + cellWidths.Sum(), origin.Y),
                rowHeight,
                rowCount + 1);

            // vertical lines
            mb.AppendParallelLines(
                new Vector2(origin.X, origin.Y + rowHeight * rowCount),
                origin,
                cellWidths);

            return mb;
        }

        //draw row content

        //public static void DrawTableContent(CanvasItem canvas, IList<(string C1, string C2)> rows, Font font, Color foreColor, Vector2 origin = default)
        //{
        //}

        //public static void DrawTable<T>(CanvasItem canvas, T dataObject, Func<T, int, int, string> dataSelector, Font font, Color lineColor, Vector2 origin = default)
        //{
        //    var rowHeight = font.GetHeight();
        //    //var rowWidths = dataSelector(dataObject, );
        //}

        /// <summary>
        /// Draw a table.
        /// </summary>
        //public static void DrawTable(CanvasItem canvas, Vector2 topLeft, IList<string> header, IList<IList<string>> data, Font font, Color lineColor)
        //{
        //    var points = new List<Vector2>((6 + data.Count + data[0].Count) * 2);

        //    var rowWidth = 200;
        //    var cellWidth = rowWidth / header.Count;
        //    var topRight = topLeft + Vector2.Right * rowWidth;
        //    var rowHeight = 20;
        //    var bodyTopLeft = topLeft + Vector2.Down * (rowHeight + 2);
        //    var bodyTopRight = bodyTopLeft + Vector2.Right * rowWidth;
        //    var tableHeight = rowHeight * (data.Count + 1) + 2;

        //    // header
        //    Multiline.AppendRectangle(points, topLeft, topRight, -rowHeight);

        //    // body
        //    for (int row = 0; row <= data.Count; row++)
        //    {
        //        var left = bodyTopLeft + row * rowHeight * Vector2.Down;
        //        Multiline.AppendLine(points, left, left + rowWidth * Vector2.Right);
        //    }

        //    for (int col = 0; col < header.Count; col++)
        //    {
        //        var top = topLeft + col * cellWidth * Vector2.Right;
        //        Multiline.AppendLine(points, top, top + tableHeight * Vector2.Down);
        //    }
        //    Multiline.AppendLine(points, topRight, topRight + tableHeight * Vector2.Down);

        //    canvas.DrawMultiline(points.ToArray(), lineColor);
        //}
    }
}
