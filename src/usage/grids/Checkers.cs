using Godot;
using GodotSharpSome.Drawing2D;
using GodotSharpSome.Grids2D;

namespace GodotSharpSome.Examples;

public partial class Checkers : OrthogonalGrid
{
	private Multiline _ml1 = Multiline.FourLineTypes();
	private Multiline _ml2 = Multiline.FourLineTypes();

	private static float FigureRadius = 18;

	public Checkers()
	{
		Options = new() { RowCount = 10, ColumnCount = 10 };
		VOptions = OrthogonalGridVOptions.CreateCheckPattern(Colors.DarkGray, Colors.LightGray);
	}

	[Export] public Color Player1Color { get; set; } = Colors.White;
	[Export] public Color Player2Color { get; set; } = Colors.Black;


    public override void _Draw()
	{
		base._Draw();
		DrawNumbers();
		//https://bonaludo.com/2019/04/04/draughts-tutorial-p-6-bridge-shot/

		DrawSetup();
		DrawMoves();
	}

	private void DrawPlayer1Figure(int column, int row)
	{
		this.DrawCircleRegion(GetTileCenter(column, row), FigureRadius, Player1Color);
		//this.DrawCircleRegion(GetCellCenter(column - 1, row - 1), 12, new Color(0.1f, 0.1f, 0.15f));
	}

    private void DrawPlayer2Figure(int column, int row)
	{
		this.DrawCircleRegion(GetTileCenter(column, row), FigureRadius, Player2Color);
	}

    private void DrawMoves()
	{
		_ml1.Clear();
		_ml2.Clear();

		AppendPlayer2Move(55, 46);
		AppendPlayer1Move(57, 35);
		AppendPlayer2Move(71, 62);
		AppendPlayer1Move(51, 73);
		AppendPlayer2Move(75, 64);
		AppendPlayer1Move(73, 55);
		AppendPlayer2Move(66, 44);
		AppendPlayer1Move(44, 8);

        DrawMultiline(_ml1.Points(), Player1Color, 3);
        DrawMultiline(_ml2.Points(), Player2Color, 2);
	}

	private void AppendPlayer1Move(int from, int to) => _ml1.AppendArrow(GetTileCenter(from) + Vector2.One * 3, GetTileCenter(to) + Vector2.One * 3);
	private void AppendPlayer2Move(int from, int to) => _ml2.AppendArrow(GetTileCenter(from) - Vector2.One * 3, GetTileCenter(to) - Vector2.One * 3);


    private void DrawSetup()
    {
        DrawPlayer1Figure(2, 1);
        DrawPlayer1Figure(4, 1);
        DrawPlayer1Figure(6, 1);
        DrawPlayer1Figure(10, 1);
        DrawPlayer1Figure(7, 2);
        DrawPlayer1Figure(8, 5);
        DrawPlayer1Figure(1, 6);
        DrawPlayer1Figure(7, 6);

        DrawPlayer2Figure(5, 6);
        DrawPlayer2Figure(6, 7);
        DrawPlayer2Figure(1, 8);
        DrawPlayer2Figure(5, 8);
        DrawPlayer2Figure(7, 8);
        DrawPlayer2Figure(3, 10);
        DrawPlayer2Figure(5, 10);
        DrawPlayer2Figure(7, 10);
    }

    private void DrawNumbers()
	{
		var number = 1;
		for (int row = 1; row <= Options.RowCount; row++)
		{
			for (int column = 1; column <= Options.RowCount; column++)
			{
				//DrawCircle(GetTileCenter(column, row), 3, Colors.Orange);
				//DrawCircle(GetTileCenter(column, row), 3, Colors.Red);
				DrawString(ThemeDB.FallbackFont, GetTileOrigin(column, row) + Vector2.Down * 10, number++.ToString(),
					HorizontalAlignment.Left, -1, 12, Colors.DarkSlateGray);
			}
		}
	}

	private Vector2 GetTileCenter(int column, int row) => GetCellCenter(column - 1, row - 1);

	private Vector2 GetTileCenter(int tileNumber) => GetCellCenter(tileNumber - 1);

    private Vector2 GetTileOrigin(int column, int row) => GetCellOrigin(column - 1, row - 1);

    private Vector2 GetTileOrigin(int tileNumber) => GetCellOrigin(tileNumber - 1);
}
