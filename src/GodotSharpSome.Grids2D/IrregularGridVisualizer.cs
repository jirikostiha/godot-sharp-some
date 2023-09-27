namespace GodotSharpSome.Grids2D
{
    using Godot;
    using GodotSharpSome.Drawing2D;
    using System.Data.Common;

    /// <summary>
    /// Irregular orthogonal grid visualizer.
    /// </summary>
    public class IrregularGridVisualizer
    {
        private readonly Multiline _mb;

        public IrregularGridVisualizer(CanvasItem canvas, OrthogonalGridVOptions? voptions = null)
        {
            Canvas = canvas;
            VOptions = voptions ?? new OrthogonalGridVOptions();
            _mb = Multiline.FourLineTypes(3, 6);
        }

        protected CanvasItem Canvas { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; set; } = new(100, 50);

        public OrthogonalGridVOptions VOptions { get; set; }

        public void Draw(IrregularGridOptions options) =>
            Draw(options.ColumnSpans, options.RowSpans);

        public void Draw(float[] columnSpans, float[] rowSpans, OrthogonalGridVOptions? voptions = null)
        {
            voptions ??= VOptions;

            if (voptions.EvenRows is not null || voptions.OddRows is not null)
            {
                // cells
                var y = Position.Y + VOptions.Overlap;
                for (int row = 0; row < rowSpans.Length; row++)
                {
                    var x = Position.X + VOptions.Overlap;
                    for (int column = 0; column < columnSpans.Length; column++)
                    {
                        if (row % 2 == 0)
                        {
                            if (column % 2 == 0)
                                Canvas.DrawRect(new Rect2(x, y, columnSpans[column], rowSpans[row]), voptions.OddRows.OddCellColor, true);
                            else
                                Canvas.DrawRect(new Rect2(x, y, columnSpans[column], rowSpans[row]), voptions.OddRows.EvenCellColor, true);
                        }
                        else
                        {
                            if (column % 2 == 0)
                                Canvas.DrawRect(new Rect2(x, y, columnSpans[column], rowSpans[row]), voptions.EvenRows.OddCellColor, true);
                            else
                                Canvas.DrawRect(new Rect2(x, y, columnSpans[column], rowSpans[row]), voptions.EvenRows.EvenCellColor, true);
                        }
                        x += columnSpans[column];
                    }
                    y += rowSpans[row];
                }
            }

            Vector2 end = Position + Size;
            Canvas.DrawCircleOutline(Position, 5, Colors.Blue);
            Canvas.DrawCircleOutline(new Vector2(end.X, Position.Y), 5, Colors.Red);
            Canvas.DrawCircleOutline(new Vector2(Position.X, end.Y), 5, Colors.Orange);

            // horizontal lines
            _mb.Clear()
                .SetPen((int)voptions.XLine.LineType)
                .AppendLine(
                    new(Position.X, Position.Y + voptions.Overlap),
                    new(end.X, Position.Y + voptions.Overlap))
                .AppendParallelLines(
                    new(Position.X, Position.Y + voptions.Overlap),
                    new(end.X, Position.Y + voptions.Overlap),
                    rowSpans)
                .AppendLine(
                    new(Position.X, end.Y - voptions.Overlap),
                    new(end.X, end.Y - voptions.Overlap));

            Canvas.DrawMultiline(_mb.Points(), voptions.XLine.Color, voptions.XLine.Width);

            // vertical lines
            _mb.Clear()
                .SetPen((int)voptions.XLine.LineType)
                .AppendLine(
                    new(Position.X + voptions.Overlap, end.Y),
                    new(Position.X + voptions.Overlap, Position.Y))
                .AppendParallelLines(
                    new(Position.X + voptions.Overlap, end.Y),
                    new(Position.X + voptions.Overlap, Position.Y),
                    columnSpans)
                .AppendLine(
                    new(end.X - voptions.Overlap, end.Y),
                    new(end.X - voptions.Overlap, Position.Y));

            Canvas.DrawMultiline(_mb.Points(), voptions.YLine.Color, voptions.YLine.Width);
        }

        //public Vector2 GetGridNodePosition(int column, int row, float overlap, float[] columnSpans, float[] rowSpans) =>
        //    new(Position.X + overlap + column * span.X,
        //        Position.Y + overlap + row * span.Y);

        //public Vector2 GetCellCenter(int column, int row, float overlap, float[] columnSpans, float[] rowSpans) =>
        //    new(Position.X + overlap + column * span.X + span.X / 2f,
        //        Position.Y + overlap + row * span.Y + span.Y / 2f);
    }
}