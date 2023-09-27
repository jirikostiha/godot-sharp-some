namespace GodotSharpSome.Grids2D
{
    using Godot;
    using GodotSharpSome.Drawing2D;

    /// <summary>
    /// Irregular orthogonal grid aligned with axis.
    /// </summary>
    public partial class IrregularGrid : ColorRect
    {
        private readonly IrregularGridVisualizer _vizer;

        public IrregularGrid()
        {
            _vizer = new(this)
            {
                Position = Vector2.Zero,
                Size = Size
            };

            Resized += SetDrawingSize;
        }

        public IrregularGridOptions Options { get; set; } = new() { ColumnSpans = new[] { 40f, 60 }, RowSpans = new[] { 30f, 50 } };

        public OrthogonalGridVOptions VOptions { get => _vizer.VOptions; set => _vizer.VOptions = value; }

        #region export

        [Export] public float[] ColumnSpans { get => Options.ColumnSpans; set => Options.ColumnSpans = value; }
        [Export] public float[] RowSpans { get => Options.RowSpans; set => Options.RowSpans = value; }

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

        #endregion

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
