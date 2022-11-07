using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Node2D_2 : Godot.Control
{
    private static Color BackColor = Color.ColorN("white");
    private static Color LineColor = Color.ColorN("black");
    private static Color AreaColor = Color.ColorN("lightgray");
    private static Color TextColor = Color.ColorN("black");
    private static VBoxContainer _container;

    private PackedScene _stripScene = GD.Load<PackedScene>("res://Strip.tscn");
    private PackedScene _cellScene = GD.Load<PackedScene>("res://Cell.tscn");

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("ScrollContainer/Rows");
        AddDrawings();
    }

    public void AddDrawings()
    {
        GD.Print(_container);

        AddDots();
        AddCrosses();
        AddArrows();
        AddSegmenedLines();
        AddVectors();
        AddCircles();
        AddTriangles();
        AddRectangles();
        AddPolygons();
        AddCandleBars();
        AddConnections();
    }

    public void AddDots()
    {
        var strip = CreateStrip("Dots");

    }

    public void AddCrosses()
    {
        var strip = CreateStrip("Crosses");

        var cell = _cellScene.Instance<Cell>();
        strip.Cells.AddChild(cell);
        //cell.RectSize = 
        cell.Title = "cell 1";
        //GodotSharpSome.Drawing2D.CanvasItemExtension.Dra
        cell.Content.DrawCircleLine(new Vector2(30, 30), 25, LineColor);
        cell.Content.Update();
    }

    public void AddArrows()
    {
        var strip = CreateStrip("Arrows");

    }

    public void AddSegmenedLines()
    {
        var strip = CreateStrip("Segmented lines");

    }

    public void AddVectors()
    {
        var strip = CreateStrip("Vectors");

    }

    public void AddCircles()
    {
        var strip = CreateStrip("Circles");

    }

    public void AddTriangles()
    {
        var strip = CreateStrip("Triangles");

    }

    public void AddRectangles()
    {
        var strip = CreateStrip("Rectangles");

    }

    public void AddPolygons()
    {
        var strip = CreateStrip("Polygons");

    }

    public void AddCandleBars()
    {
        var strip = CreateStrip("CandleBars");

    }

    public void AddConnections()
    {
        var strip = CreateStrip("Connections");

    }

    private Strip CreateStrip(string groupName)
    {
        var strip = _stripScene.Instance<Strip>();
        _container.AddChild(strip);
        strip.GroupName = groupName;

        return strip;
    }
}
