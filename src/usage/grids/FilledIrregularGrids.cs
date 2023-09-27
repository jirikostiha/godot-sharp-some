using Godot;
using GodotSharpSome.Drawing2D.Examples;

namespace GodotSharpSome.Grids2D.Examples;

public partial class FilledIrregularGrids : ExampleNodeBase
{
	private IrregularGridVisualizer _vizer;

	private OrthogonalGridVOptions _columnGridVoptions;

	private OrthogonalGridVOptions _rowGridVoptions;

	private OrthogonalGridVOptions _checkGridVoptions;

	private OrthogonalGridVOptions _checkGridOverlapedVoptions;

	private IrregularGridOptions _someGrid;

	public FilledIrregularGrids()
	{
		_vizer = new(this)
		{
			Size = CellSize
		};

		_columnGridVoptions = OrthogonalGridVOptions.CreateColumnPattern(Colors.Wheat, Colors.DarkOrange);
		_columnGridVoptions.XLine.Color = LineColor;
		_columnGridVoptions.YLine.Color = LineColor;

		_rowGridVoptions = OrthogonalGridVOptions.CreateRowPattern(Colors.LightSeaGreen);
		_rowGridVoptions.XLine.Color = LineColor;
		_rowGridVoptions.YLine.Color = LineColor;

		_checkGridVoptions = OrthogonalGridVOptions.CreateCheckPattern(Colors.Black, Colors.White);
		_checkGridVoptions.XLine.Color = LineColor;
		_checkGridVoptions.YLine.Color = LineColor;

		_checkGridOverlapedVoptions = OrthogonalGridVOptions.CreateCheckPattern(Colors.Black, Colors.White);
		_checkGridOverlapedVoptions.XLine.Color = LineColor;
		_checkGridOverlapedVoptions.YLine.Color = LineColor;
		_checkGridOverlapedVoptions.Overlap = 10;

		_someGrid = new() { ColumnSpans = new[] { 8f, 12, 18, 22 }, RowSpans = new[] { 10f, 15, 20 } };
	}

	public override void _Draw()
	{
		DrawColumnGrid(LeftBottom(1));

		DrawRowGrid(LeftBottom(2));

		DrawCheckGrid(LeftBottom(3));

		DrawCheckGridOverlaped(LeftBottom(4));
	}

	private void DrawColumnGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _columnGridVoptions;

		_vizer.Draw(_someGrid);
	}

	private void DrawRowGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _rowGridVoptions;

		_vizer.Draw(_someGrid);
	}

	private void DrawCheckGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _checkGridVoptions;

		_vizer.Draw(_someGrid);
	}

	private void DrawCheckGridOverlaped(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _checkGridOverlapedVoptions;

		_vizer.Draw(_someGrid);
	}
}
