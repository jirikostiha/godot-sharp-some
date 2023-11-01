using Godot;
using GodotSharpSome.Drawing2D;
using GodotSharpSome.Drawing2D.Examples;

namespace GodotSharpSome.Grids2D.Examples;

public partial class WiredIrregularGrids : ExampleNodeBase
{
	private IrregularGridVisualizer _vizer;

	private OrthogonalGridVOptions _solidGridVoptions;

	private OrthogonalGridVOptions _solidOverlapGridVoptions;

	private OrthogonalGridVOptions _dottedGridVoptions;

	private OrthogonalGridVOptions _dashedGridVoptions;

	private IrregularGridOptions _oneByOneGrid;

	private IrregularGridOptions _someGrid;

	public WiredIrregularGrids()
	{
		_vizer = new(this)
		{
			Size = CellSize
		};

		_solidGridVoptions = new()
		{
			XLine = new() { LineType = LineType.Solid, Color = LineColor, Width = 1 },
			YLine = new() { LineType = LineType.Solid, Color = LineColor, Width = 2 },
		};

		_solidOverlapGridVoptions = new()
		{
			XLine = new() { LineType = LineType.Solid, Color = LineColor, Width = 1 },
			YLine = new() { LineType = LineType.Solid, Color = LineColor, Width = 1 },
			Overlap = 15,
		};

		_dashedGridVoptions = new()
		{
			XLine = new() { LineType = LineType.Dashed, Color = LineColor, Width = 1 },
			YLine = new() { LineType = LineType.Dashed, Color = LineColor, Width = 1 },
		};

		_dottedGridVoptions = new()
		{
			XLine = new() { LineType = LineType.Dotted, Color = LineColor, Width = 2 },
			YLine = new() { LineType = LineType.Dotted, Color = LineColor, Width = 2 },
			Overlap = 10,
		};

		_oneByOneGrid = new() { ColumnSpansPrio = new float[0], RowSpansPrio = new float[0] };

		_someGrid = new() { ColumnSpansPrio = new[] { 10f, 25 }, RowSpansPrio = new[] { 10f, 20, 20 } };
	}

	public override void _Draw()
	{
		DrawSolidGrid1x1(LeftBottom(1));

		DrawSolidGrid1x1Overlap(LeftBottom(2));

		DrawSolidGrid(LeftBottom(3));

		DrawDashedGrid(LeftBottom(4));

		DrawDottedGrid(LeftBottom(5));
	}

	private void DrawSolidGrid1x1(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _solidGridVoptions;

		_vizer.Draw(_oneByOneGrid);
	}

	private void DrawSolidGrid1x1Overlap(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _solidOverlapGridVoptions;

		_vizer.Draw(_oneByOneGrid);
	}

	private void DrawSolidGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _solidGridVoptions;

		_vizer.Draw(_someGrid);
	}

	private void DrawDashedGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _dashedGridVoptions;

		_vizer.Draw(_someGrid);
	}

	private void DrawDottedGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _dottedGridVoptions;

		_vizer.Draw(_someGrid);
	}
}
