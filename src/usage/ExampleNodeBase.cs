using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class ExampleNodeBase : Godot.ColorRect
{
    protected static Color BackColor = Color.ColorN("white");
    protected static Color TextColor = Color.ColorN("black");
    protected static Color LineColor = Color.ColorN("black");
    protected static Color AreaColor = Color.ColorN("gray");

    protected static int Margin = 10;
    protected static int RowHeight = 100;
    protected static int CellWidth = 100;

    public override void _Ready()
    {
        Color = BackColor;
    }

    public Vector2 LeftBottom(int column) => new Vector2(Margin + (column - 1) * CellWidth, Margin);
    
    public Vector2 LeftTop(int column) => new Vector2(Margin + (column - 1) * CellWidth, RowHeight - Margin);

    public Vector2 RightBottom(int column) => new Vector2(column * CellWidth - Margin, Margin);

    public Vector2 RightTop(int column) => new Vector2(column * CellWidth - Margin, RowHeight - Margin);

    public Vector2 Middle(int column) => new Vector2(Convert.ToInt32((column - 0.5) * CellWidth), Convert.ToInt32(0.5 * RowHeight));

    public Vector2 LeftMiddle(int column) => new Vector2(Margin + (column - 1) * CellWidth, Convert.ToInt32(0.5 * RowHeight));

    public Vector2 RightMiddle(int column) => new Vector2(column * CellWidth - Margin, Convert.ToInt32(0.5 * RowHeight));
    
    public Vector2 MiddleTop(int column) => new Vector2(Convert.ToInt32((column - 0.5) * CellWidth), RowHeight - Margin);

    public Vector2 MiddleBottom(int column) => new Vector2(Convert.ToInt32((column - 0.5) * CellWidth), Margin);
}
