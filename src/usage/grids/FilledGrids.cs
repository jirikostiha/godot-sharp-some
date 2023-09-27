using Godot;
using GodotSharpSome.Drawing2D.Examples;

namespace GodotSharpSome.Grids2D.Examples;

public partial class FilledGrids : ExampleNodeBase
{
	private OrthogonalGridVisualizer _vizer;

	private OrthogonalGridVOptions _collumnGridVoptions;

	private OrthogonalGridVOptions _rowGridVoptions;

	private OrthogonalGridVOptions _checkGridVoptions;

    private OrthogonalGridVOptions _checkGridOverlapedVoptions;

    public FilledGrids()
	{
		_vizer = new(this)
		{
			Size = CellSize
		};

		_collumnGridVoptions = OrthogonalGridVOptions.CreateColumnPattern(Colors.Wheat, Colors.DarkOrange);
		_collumnGridVoptions.XLine.Color = LineColor;
		_collumnGridVoptions.YLine.Color = LineColor;

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
    }

	public override void _Draw()
	{
		DrawCollumnGrid(LeftBottom(1));

		DrawRowGrid(LeftBottom(2));

		DrawCheckGrid(LeftBottom(3));

		DrawCheckGridOverlaped(LeftBottom(4));
	}

	private void DrawCollumnGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _collumnGridVoptions;

		_vizer.Draw(5, 4);
	}

	private void DrawRowGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _rowGridVoptions;

		_vizer.Draw(5, 4);
	}

	private void DrawCheckGrid(Vector2 origin)
	{
		_vizer.Position = origin;
		_vizer.VOptions = _checkGridVoptions;

		_vizer.Draw(5, 4);
	}

    private void DrawCheckGridOverlaped(Vector2 origin)
    {
        _vizer.Position = origin;
        _vizer.VOptions = _checkGridOverlapedVoptions;

        _vizer.Draw(5, 4);
    }
}
