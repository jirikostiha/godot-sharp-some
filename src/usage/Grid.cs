using System;
using Godot;

internal class Grid
{
    private int _margin = 10;
    private int _rowSize = 100;
    private int _cellWidth = 100;

    public Grid(int margin, int rowSize, int cellWidth)
    {
        _margin = margin;
        _rowSize = rowSize;
        _cellWidth = cellWidth;
    }

    public Vector2 LeftBottom(int row, int column) => new Vector2(LeftBottomX(column), LeftBottomY(row));
    public int LeftBottomX(int column) => _margin + (column - 1) * _cellWidth;
    public int LeftBottomY(int row) => _margin + (row - 1) * _rowSize;

    public Vector2 LeftTop(int row, int column) => new Vector2(LeftTopX(column), LeftTopY(row));
    public int LeftTopX(int column) => _margin + (column - 1) * _cellWidth;
    public int LeftTopY(int row) => row * _rowSize - _margin;

    public Vector2 RightBottom(int row, int column) => new Vector2(RightBottomX(column), RightBottomY(row));
    public int RightBottomX(int column) => column * _cellWidth - _margin;
    public int RightBottomY(int row) => _margin + (row - 1) * _rowSize;

    public Vector2 RightTop(int row, int column) => new Vector2(RightTopX(column), RightTopY(row));
    public int RightTopX(int column) => column * _cellWidth - _margin;
    public int RightTopY(int row) => row * _rowSize - _margin;

    public Vector2 Middle(int row, int column) => new Vector2(MiddleX(column), MiddleY(row));
    public int MiddleX(int column) => Convert.ToInt32((column - 0.5) * _cellWidth);
    public int MiddleY(int row) => Convert.ToInt32((row - 0.5) * _rowSize);
    
    public Vector2 LeftMiddle(int row, int column) => new Vector2(LeftMiddleX(column), LeftMiddleY(row));
    public int LeftMiddleX(int column) => _margin + (column - 1) * _cellWidth;
    public int LeftMiddleY(int row) => Convert.ToInt32((row - 0.5) * _rowSize);

    public Vector2 RightMiddle(int row, int column) => new Vector2(RightMiddleX(column), RightMiddleY(row));
    public int RightMiddleX(int column) => column * _cellWidth - _margin;
    public int RightMiddleY(int row) => Convert.ToInt32((row - 0.5) * _rowSize);

    public Vector2 TopMiddle(int row, int column) => new Vector2(TopMiddleX(column), TopMiddleY(row));
    public int TopMiddleX(int column) => Convert.ToInt32((column - 0.5) * _cellWidth);
    public int TopMiddleY(int row) => row * _rowSize - _margin;

    public Vector2 BottomMiddle(int row, int column) => new Vector2(BottomMiddleX(column), BottomMiddleY(row));
    public int BottomMiddleX(int column) => Convert.ToInt32((column - 0.5) * _cellWidth);
    public int BottomMiddleY(int row) => _margin + (row - 1) * _rowSize;
}
