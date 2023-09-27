namespace GodotSharpSome.Grids2D
{
    using Godot;
    using GodotSharpSome.Drawing2D;

    /// <summary>
    /// Regular orthogonal grid aligned with axis.
    /// </summary>
    public partial class OrthogonalGrid : ColorRect
    {
        private readonly OrthogonalGridVisualizer _vizer;

        public OrthogonalGrid()
        {
            _vizer = new(this)
            {
                Position = Vector2.Zero,
                Size = Size
            };

            Resized += SetDrawingSize;
        }

        public OrthogonalGridOptions Options { get; set; } = new() { ColumnCount = 2, RowCount = 2 };

        public OrthogonalGridVOptions VOptions { get => _vizer.VOptions; set => _vizer.VOptions = value; }

        #region export

        [Export] public int XCount { get => Options.RowCount; set => Options.RowCount = value; }
        [Export] public int YCount { get => Options.ColumnCount; set => Options.ColumnCount = value; }

        [Export] public float Margin { get; set; } = 10f;

        [Export] public Color XLineColor { get => _vizer.VOptions.XLine.Color; set => _vizer.VOptions.XLine.Color = value; }
        [Export] public float XLineWidth { get => _vizer.VOptions.XLine.Width; set => _vizer.VOptions.XLine.Width = value; }
        [Export] public LineType XLineType { get => _vizer.VOptions.XLine.LineType; set => _vizer.VOptions.XLine.LineType = value; }

        [Export] public Color YLineColor { get => _vizer.VOptions.YLine.Color; set => _vizer.VOptions.YLine.Color = value; }
        [Export] public float YLineWidth { get => _vizer.VOptions.YLine.Width; set => _vizer.VOptions.YLine.Width = value; }
        [Export] public LineType YLineType { get => _vizer.VOptions.YLine.LineType; set => _vizer.VOptions.YLine.LineType = value; }

        [Export] public Color OddRowOddCellColor
        {
            get => _vizer.VOptions.OddRows?.OddCellColor ?? default;
            set
            {
                if (_vizer.VOptions.OddRows is null)
                    _vizer.VOptions.OddRows = new();
                _vizer.VOptions.OddRows.OddCellColor = value;
            }
        }
        [Export] public Color OddRowEvenCellColor
        {
            get => _vizer.VOptions.OddRows?.EvenCellColor ?? default;
            set
            {
                if (_vizer.VOptions.OddRows is null)
                    _vizer.VOptions.OddRows = new();
                _vizer.VOptions.OddRows.EvenCellColor = value;
            }
        }
        [Export] public Color EvenRowOddCellColor
        {
            get => _vizer.VOptions.EvenRows?.OddCellColor ?? default;
            set
            {
                if (_vizer.VOptions.EvenRows is null)
                    _vizer.VOptions.EvenRows = new();
                _vizer.VOptions.EvenRows.OddCellColor = value;
            }
        }

        [Export] public Color EvenRowEvenCellColor
        {
            get => _vizer.VOptions.EvenRows?.EvenCellColor ?? default;
            set
            {
                if (_vizer.VOptions.EvenRows is null)
                    _vizer.VOptions.EvenRows = new();
                _vizer.VOptions.EvenRows.EvenCellColor = value;
            }
        }

        //[Export]
        //public float XSpan { get => (Size.X - 2 * Margin) / Options.RowCount; }
        ////[Export]
        //public float YSpan { get => (Size.Y - 2 * Margin) / Options.ColumnCount; }

        #endregion

        /// <summary>
        /// Get position of a cell corner with lowest values of x and y.
        /// </summary>
        //public Vector2 GetCellOrigin(int column, int row) =>
        //    new(Position.X + VOptions.Overlap + column * _vizer.GetXSpan(Options.ColumnCount, VOptions.Overlap),
        //        Position.Y + VOptions.Overlap + row * _vizer.GetYSpan(Options.RowCount, VOptions.Overlap));

        public Vector2 GetCellOrigin(int column, int row) =>
            new(_vizer.GetCellOriginX(column, VOptions.Overlap, _vizer.GetXSpan(Options.ColumnCount, VOptions.Overlap)),
                _vizer.GetCellOriginY(row, VOptions.Overlap, _vizer.GetYSpan(Options.RowCount, VOptions.Overlap)));

        /// <summary>
        /// Get position of a cell corner with lowest values of x and y.
        /// </summary>
        public Vector2 GetCellOrigin(int cellNumber) =>
            GetCellOrigin(cellNumber % Options.ColumnCount, cellNumber / Options.RowCount);

        public Vector2 GetGridNodePosition2(int column, int row) =>
            _vizer.GetCellOrigin(column, row, VOptions.Overlap,
                new Vector2(
                    _vizer.GetXSpan(Options.ColumnCount, VOptions.Overlap),
                    _vizer.GetYSpan(Options.RowCount, VOptions.Overlap)));


        /// <summary>
        /// Get position of cell center.
        /// </summary>
        public Vector2 GetCellCenter(int column, int row) =>
            _vizer.GetCellCenter(column, row, VOptions.Overlap, _vizer.GetSpan(Options.ColumnCount, Options.RowCount, VOptions.Overlap));

        /// <summary>
        /// Get position of cell center by cell number.
        /// </summary>
        public Vector2 GetCellCenter(int cellNumber) =>
            _vizer.GetCellCenter(CellNumberToColumn(cellNumber), CellNumberToRow(cellNumber), VOptions.Overlap, _vizer.GetSpan(Options.ColumnCount, Options.RowCount));

        public int CellNumberToColumn(int cellNumber) => cellNumber % Options.ColumnCount;
        public int CellNumberToRow(int cellNumber) => cellNumber / Options.ColumnCount;

        public override void _Draw()
        {
            _vizer.Draw(Options);
        }

        private void SetDrawingSize()
        {
            _vizer.Size = Size;
        }
    }
}
