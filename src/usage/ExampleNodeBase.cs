using Godot;
using System;

namespace GodotSharpSome.Drawing2D.Examples;

public partial class ExampleNodeBase : ColorRect
{
    public Color TextColor { get; set; } = Colors.Black;
    public Color LineColor { get; set; } = Colors.Black;
    public Color LineColor2 { get; set; } = Colors.Orange;
    public Color AreaColor { get; set; } = Colors.Gray;

    protected static int Margin { get; set; } = 10;
    protected static int RawRowHeight { get; set; } = 100;
    protected static int RowHeight => RawRowHeight - 2 * Margin;
    protected static int RawCellWidth { get; set; } = 100;
    protected static int CellWidth => RawCellWidth - 2 * Margin;

    protected static Vector2 CellWidthVector => new(CellWidth, 0);
    protected static Vector2 RowHeightVector => new(0, RowHeight);

    protected static Vector2 RawCellSize => new(RawCellWidth, RawRowHeight);
    protected static Vector2 CellSize => new(CellWidth, RowHeight);

    protected static RandomNumberGenerator Rng { get; set; } = new();

    protected Font Font { get; set; } = ThemeDB.FallbackFont;

    public bool Animate { get; set; }

    public bool Invert { get; set; }

    public override void _Ready()
    {
        Color = ExampleList.LightBack;
        Font = ThemeDB.FallbackFont;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Animate)
        {
            NextState(delta);
            QueueRedraw();
        }
    }

    protected virtual void NextState(double delta)
    { }

    public static float NextUin() => Rng.Randf();

    public static float NextFloat(float inclusiveMin, float exclusiveMax)
        => inclusiveMin + NextUin() * (exclusiveMax - inclusiveMin);

    public static int NextInt(int min, int max) => min + (int)(NextUin() * (max + 1 - min));

    public static bool NextBool() => NextUin() >= 0.5f;

    public Color NextColor(int inclusiveMin, int exclusiveMax) => new(
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax));

    public Color NextColorWithAlpha(float inclusiveMin, float exclusiveMax) => new(
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax));

    public static Vector2 NextVector(float xMin, float xMax, float yMin, float yMax) => new(
        NextFloat(xMin, xMax),
        NextFloat(yMin, yMax));

    public static Vector2 NextVectorBetween(Vector2 a, Vector2 b) => new(
        NextFloat(a.X, b.X),
        NextFloat(a.Y, b.Y));

    public Vector2 NextVectorInsideCell(int column) => new(
        NextInt(Left(column), Right(column) + 1),
        NextInt(Bottom(), Top()));

    public int Left(int column) => Margin + (column - 1) * RawCellWidth;

    public int Right(int column) => column * RawCellWidth - Margin;

    public int Top() => RowHeight - Margin;

    public int Bottom() => Margin;

    public int MiddleX(int column) => Convert.ToInt32((column - 0.5) * RawCellWidth);

    public int MiddleY() => Convert.ToInt32(0.5 * RowHeight);

    public Vector2 LeftBottom(int column) => new(Left(column), Bottom());

    public Vector2 LeftTop(int column) => new(Left(column), Top());

    public Vector2 RightBottom(int column) => new(Right(column), Bottom());

    public Vector2 RightTop(int column) => new(Right(column), Top());

    public Vector2 LeftMiddle(int column) => new(Left(column), MiddleY());

    public Vector2 RightMiddle(int column) => new(Right(column), MiddleY());

    public Vector2 MiddleTop(int column) => new(MiddleX(column), Top());

    public Vector2 MiddleBottom(int column) => new(MiddleX(column), Bottom());

    public Vector2 Middle(int column) => new(MiddleX(column), MiddleY());

    public void _on_Animate_pressed() => Animate = !Animate;

    public void _on_Invert_pressed() => InvertColors();

    public void InvertColors()
    {
        Invert = !Invert;
        if (!Invert)
        {
            Color = ExampleList.LightBack;
            LineColor = ExampleList.DarkStroke;
            AreaColor = ExampleList.LightRegion;
            TextColor = ExampleList.DarkStroke;
        }
        else
        {
            Color = ExampleList.DarkBack;
            LineColor = ExampleList.LightStroke;
            AreaColor = ExampleList.DarkRegion;
            TextColor = ExampleList.LightStroke;
        }
    }
}