namespace GodotSharpSome.Grids2D
{
    using Godot;
    using GodotSharpSome.Drawing2D;

    /// <summary>
    /// Regular orthogonal grid visualizer.
    /// </summary>
    public class OrthogonalGridVisualizer
    {
        private readonly Multiline _mb;

        public OrthogonalGridVisualizer(CanvasItem canvas, OrthogonalGridVOptions? voptions = null)
        {
            Canvas = canvas;
            VOptions = voptions ?? new OrthogonalGridVOptions();
            _mb = Multiline.FourLineTypes(3, 6);
        }

        protected CanvasItem Canvas { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; set; } = new(100, 50);

        public OrthogonalGridVOptions VOptions { get; set; }

        public void Draw(OrthogonalGridOptions options) =>
            Draw(options.ColumnCount, options.RowCount);

        public void Draw(int columns, int rows, OrthogonalGridVOptions? voptions = null)
        {
            voptions ??= VOptions;

            Vector2 span = GetSpan(columns, rows, voptions.Overlap);

            if (voptions.EvenRows is not null || voptions.OddRows is not null)
            {
                // cells
                for (int row = 0; row < rows; row++)
                    for (int column = 0; column < columns; column++)
                    {
                        if (row % 2 == 0)
                        {
                            if (column % 2 == 0)
                                Canvas.DrawRectangleRegion(
                                    GetCellOrigin(column + 1, row, voptions.Overlap, span),
                                    GetCellOrigin(column, row, voptions.Overlap, span),
                                    span.Y,
                                    voptions.OddRows.OddCellColor);
                            else
                                Canvas.DrawRectangleRegion(
                                    GetCellOrigin(column + 1, row, voptions.Overlap, span),
                                    GetCellOrigin(column, row, voptions.Overlap, span),
                                    span.Y,
                                    voptions.OddRows.EvenCellColor);
                        }
                        else
                        {
                            if (column % 2 == 0)
                                Canvas.DrawRectangleRegion(
                                    GetCellOrigin(column + 1, row, voptions.Overlap, span),
                                    GetCellOrigin(column, row, voptions.Overlap, span),
                                    span.Y,
                                    voptions.EvenRows.OddCellColor);
                            else
                                Canvas.DrawRectangleRegion(
                                    GetCellOrigin(column + 1, row, voptions.Overlap, span),
                                    GetCellOrigin(column, row, voptions.Overlap, span),
                                    span.Y,
                                    voptions.EvenRows.EvenCellColor);
                        }
                    }
            }

            Vector2 end = Position + Size;
            //Canvas.DrawCircleOutline(Position, 5, Colors.Blue);
            //Canvas.DrawCircleOutline(new Vector2(end.X, Position.Y), 5, Colors.Red);
            //Canvas.DrawCircleOutline(new Vector2(Position.X, end.Y), 5, Colors.Orange);

            // horizontal lines
            _mb.Clear()
                .SetPen((int)voptions.XLine.LineType)
                .AppendParallelLines(
                    new (Position.X, Position.Y + voptions.Overlap),
                    new (end.X, Position.Y + voptions.Overlap),
                    span.Y,
                    rows + 1);

            Canvas.DrawMultiline(_mb.Points(), voptions.XLine.Color, voptions.XLine.Width);

            // vertical lines
            _mb.Clear()
                .SetPen((int)voptions.XLine.LineType)
                .AppendParallelLines(
                    new(Position.X + voptions.Overlap, end.Y),
                    new(Position.X + voptions.Overlap, Position.Y),
                    span.X,
                    columns + 1);

            Canvas.DrawMultiline(_mb.Points(), voptions.YLine.Color, voptions.YLine.Width);
        }

        public float GetXSpan(int columns, float overlap) => (Size.X - 2 * overlap) / columns;
        public float GetXSpan(int columns) => (Size.X - 2 * VOptions.Overlap) / columns;

        public float GetYSpan(int rows, float overlap) => (Size.Y - 2 * overlap) / rows;
        public float GetYSpan(int rows) => (Size.Y - 2 * VOptions.Overlap) / rows;

        public Vector2 GetSpan(int columns, int rows, float overlap) => new(GetXSpan(columns, overlap), GetYSpan(rows, overlap));

        public Vector2 GetSpan(int columns, int rows) => new(GetXSpan(columns), GetYSpan(rows));

        public Vector2 GetCellOrigin(int column, int row, float overlap, Vector2 span) =>
            new(GetCellOriginX(column, overlap, span.X), GetCellOriginY(row, overlap, span.Y));

        public float GetCellOriginX(int column, float overlap, float spanX) => Position.X + overlap + column * spanX;
        public float GetCellOriginY(int row, float overlap, float spanY) => Position.Y + overlap + row * spanY;

        public Vector2 GetCellCenter(int column, int row, float overlap, Vector2 span) =>
            new(GetCellCenterX(column, overlap, span.X), GetCellCenterY(row, overlap, span.Y));

        public float GetCellCenterX(int column, float overlap, float spanX) => Position.X + overlap + column * spanX + spanX / 2f;
        public float GetCellCenterY(int row, float overlap, float spanY) => Position.Y + overlap + row * spanY + spanY / 2f;
    }
}