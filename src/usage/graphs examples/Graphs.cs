using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using GodotSharpSome.Drawing2D.Examples;

namespace GodotSharpSome.Graphs2D.Examples;

public partial class Graphs : ExampleNodeBase
{
    private readonly List<int[]> _perceptron;
    private readonly List<int[]> _twoOne;
    private readonly List<int[]> _oneTwo;
    private readonly List<int[]> _net;

    private readonly LayeredNetworkVisualizer _vizer;

    public Graphs()
    {
        _perceptron = CreateNaiveNet(1);
        _twoOne = CreateNaiveNet(2, 1);
        _oneTwo = CreateNaiveNet(1, 2);
        _net = CreateNaiveNet(2, 3, 2);

        _vizer = new LayeredNetworkVisualizer(this)
        {
            Size = CellSize,
            ConnectionVOptions = new()
            {
                Line = new() { LineType = LineType.Solid, Width = 1, Color = Colors.DarkGray },
            },
        };

        _vizer.InputNodeVizer.VOptions = new()
        {
            FillColor = Colors.LightBlue,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
        _vizer.NodeVizer.VOptions = new()
        {
            FillColor = Colors.Gray,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
        _vizer.OutputNodeVizer.VOptions = new()
        {
            FillColor = Colors.Green,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
    }

    public override void _Draw()
    {
        DrawPerceptron(LeftBottom(1));

        DrawTwoOne(LeftBottom(2));

        DrawOneTwo(LeftBottom(3));

        DrawNet1(LeftBottom(4));
    }

    private void DrawPerceptron(Vector2 origin)
    {
        _vizer.Position = origin;
        DrawRect(new Rect2(_vizer.Position, _vizer.Size), Colors.LightGray);
        _vizer.Draw(_perceptron);
    }

    private void DrawTwoOne(Vector2 origin)
    {
        _vizer.Position = origin;
        DrawRect(new Rect2(_vizer.Position, _vizer.Size), Colors.LightGray);
        _vizer.Draw(_twoOne);
    }

    private void DrawOneTwo(Vector2 origin)
    {
        _vizer.Position = origin;
        DrawRect(new Rect2(_vizer.Position, _vizer.Size), Colors.LightGray);
        _vizer.Draw(_oneTwo);
    }

    private void DrawNet1(Vector2 origin)
    {
        _vizer.Position = origin;
        DrawRect(new Rect2(_vizer.Position, _vizer.Size), Colors.LightGray);
        _vizer.Draw(_net);
    }

    internal List<int[]> CreateNaiveNet(params int[] layerSizes)
    {
        int couter = 1;
        var layers = new List<int[]>();
        for (int layerIndex = 0; layerIndex < layerSizes.Length; layerIndex++)
        {
            var layer = new List<int>();
            for (int nodeIndex = 0; nodeIndex < layerSizes[layerIndex]; nodeIndex++)
                layer.Add(couter++);

            layers.Add(layer.ToArray());
        }
        return layers;
    }
}
