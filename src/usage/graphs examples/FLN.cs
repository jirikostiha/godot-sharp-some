using Godot;
using GodotSharpSome.Drawing2D;
using System.Collections.Generic;
using GodotSharpSome.Drawing2D.Examples;
using static GodotSharpSome.Graphs2D.Examples.FLN;
using System.Globalization;

namespace GodotSharpSome.Graphs2D.Examples;

internal partial class FLN : LayeredNetwork<NaiveNode>
{
    private float _nodeRadius = 10;

    internal FLN()
    {
        Layers = CreateNaiveNet(10, 6, 5, 8);

        ConnectionVoptions = new()
        {
            Line = new() { LineType = LineType.Solid, Width = 1, Color = Colors.DarkGray },
        };
        InputNodeVOptions = new()
        {
            FillColor = Colors.LightBlue,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
        NodeVOptions = new()
        {
            FillColor = Colors.Gray,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
        OutputNodeVOptions = new()
        {
            FillColor = Colors.Green,
            ValueColor = Colors.Black,
            Outline = new LineVOptions() { Color = Colors.Black },
        };
    }

    public override void _Ready()
    {
        Color = ExampleList.LightBack;
    }

    public override void _PhysicsProcess(double delta)
    {
        Mutate(Layers);
        QueueRedraw();
    }

    internal void Mutate(IList<NaiveNode[]> layers)
    {
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].Length; nodeIndex++)
            {
                layers[layerIndex][nodeIndex].Value += ExampleNodeBase.NextFloat(-0.2f, 0.2f);
            }
        }
    }

    internal List<NaiveNode[]> CreateNaiveNet(params int[] layerSizes)
    {
        var counter = 1;
        var net = new List<NaiveNode[]>();

        foreach (var layerSize in layerSizes)
        {
            var layer = new NaiveNode[layerSize];
            for (int i = 0; i < layerSize; i++)
                layer[i] = new NaiveNode(counter++);

            net.Add(layer);
        }

        return net;
    }

    internal class NaiveNode
    {
        public NaiveNode(int number)
        {
            Number = number;
            Value = number / 10f;
        }

        public int Number { get; private set; }

        public float Value { get; set; }

        public override string ToString() => Value.ToString("0.0", CultureInfo.InvariantCulture);
    }
}