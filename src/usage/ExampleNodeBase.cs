using System;
using System.Linq;
using Godot;

public class ExampleNodeBase : Godot.ColorRect
{
    public Color TextColor = Color.ColorN("black");
    public Color LineColor = Color.ColorN("black");
    public Color LineColor2 = Color.ColorN("orange");
    public Color AreaColor = Color.ColorN("gray");

    protected static int Margin { get; set; } = 10;
    protected static int RowHeight { get; set; } = 100;
    protected static int CellWidth { get; set; } = 100;

    protected static Vector2 CellWidthVector => new Vector2(CellWidth, 0);

    protected static RandomNumberGenerator Rng { get; set; } = new RandomNumberGenerator();

    public bool Animate { get; set; }

    public bool Inverse { get; set; }

    public override void _Ready()
    {
        Color = ExampleList.LightBack;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Animate)
        {
            NextState(delta);
            Update();
        }
    }

    protected virtual void NextState(float delta) { }

    public float NextUin() => Rng.Randf();

    public float NextFloat(float inclusiveMin, float exclusiveMax) => inclusiveMin + NextUin() * (exclusiveMax - inclusiveMin);

    public int NextInt(int min, int max) => min + (int)(NextUin() * (max + 1 - min));

    public bool NextBool() => NextUin() >= 0.5f;

    public Color NextColor(int inclusiveMin, int exclusiveMax) => new(
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax));

    public Color NextColorWithAlpha(float inclusiveMin, float exclusiveMax) => new(
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax),
        NextFloat(inclusiveMin, exclusiveMax));

    public Vector2 NextVector(float xMin, float xMax, float yMin, float yMax) => new Vector2(
        NextFloat(xMin, xMax),
        NextFloat(yMin, yMax));

    public Vector2 NextVectorBetween(Vector2 a, Vector2 b) => new Vector2(
        NextFloat(a.x, b.x),
        NextFloat(a.y, b.y));

    public Vector2 NextVectorInsideCell(int column) => new Vector2(
        NextInt(Left(column), Right(column) + 1),
        NextInt(Bottom(), Top()));

    public int Left(int column) => Margin + (column - 1) * CellWidth;

    public int Right(int column) => column * CellWidth - Margin;

    public int Top() => RowHeight - Margin;

    public int Bottom() => Margin;

    public int MiddleX(int column) => Convert.ToInt32((column - 0.5) * CellWidth);

    public int MiddleY() => Convert.ToInt32(0.5 * RowHeight);

    public Vector2 LeftBottom(int column) => new Vector2(Left(column), Bottom());

    public Vector2 LeftTop(int column) => new Vector2(Left(column), Top());

    public Vector2 RightBottom(int column) => new Vector2(Right(column), Bottom());

    public Vector2 RightTop(int column) => new Vector2(Right(column), Top());

    public Vector2 LeftMiddle(int column) => new Vector2(Left(column), MiddleY());

    public Vector2 RightMiddle(int column) => new Vector2(Right(column), MiddleY());

    public Vector2 MiddleTop(int column) => new Vector2(MiddleX(column), Top());

    public Vector2 MiddleBottom(int column) => new Vector2(MiddleX(column), Bottom());

    public Vector2 Middle(int column) => new Vector2(MiddleX(column), MiddleY());

    public void _on_Inverse_pressed() => InverseColors();

    public void InverseColors()
    {
        Inverse = !Inverse;
        if (!Inverse)
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
