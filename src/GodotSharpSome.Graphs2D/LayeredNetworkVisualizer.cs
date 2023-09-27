using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GodotSharpSome.Graphs2D.Tests")]

namespace GodotSharpSome.Graphs2D;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class LayeredNetworkVisualizer
{
    private readonly Multiline _mb;

    public LayeredNetworkVisualizer(Control canvas)
    {
        Canvas = canvas;
        ConnectionVOptions = new();
        NodeVizer = new(canvas);
        InputNodeVizer = new(canvas);
        OutputNodeVizer = new(canvas);
        _mb = Multiline.FourLineTypes();
    }

    protected Control Canvas { get; private set; }

    public Vector2 Position { get; set; }

    public Vector2 Size { get; set; } = new (250, 200);

    public ConnectionVoptions ConnectionVOptions { get; set; }

    public NodeVisualizer InputNodeVizer { get; set; }

    public NodeVisualizer NodeVizer { get; set; }

    public NodeVisualizer OutputNodeVizer { get; set; }

    public void Draw<TNode>(IList<TNode[]> layers)
    {
        if (layers.Count == 0)
            return;

        //GD.Print("---------" + layers.Count);
        //GD.Print("layers " + layers.Count);
        var maxNodesInLayer = layers.Max(l => l.Length);
        //GD.Print("maxNodesInLayer " + maxNodesInLayer);
        var nodeRadius = MathF.Min(GetNodeRadius(layers.Count, maxNodesInLayer), InputNodeVizer.VOptions.MaxNodeRadius);
        //GD.Print("nodeRadius " + nodeRadius);

        InputNodeVizer.NodeRadius = nodeRadius;
        NodeVizer.NodeRadius = nodeRadius;
        OutputNodeVizer.NodeRadius = nodeRadius;
        DrawNodes(layers, maxNodesInLayer);
        AppendConnections(layers, maxNodesInLayer);

        DrawConnections();
    }

    internal void DrawNodes<TNode>(IList<TNode[]> layers, int maxNodesInLayer)
    {
        //draw input nodes
        for (int nodeIndex = 0; nodeIndex < layers[0].Length; nodeIndex++)
        {
            InputNodeVizer.Draw(
                layers[0][nodeIndex],
                new Vector2(
                    GetNodeXCoord(nodeIndex, layers[0].Length, InputNodeVizer.NodeRadius, maxNodesInLayer),
                    GetNodeYCoord(0, layers.Count, InputNodeVizer.NodeRadius)));
        }

        if (layers.Count == 1)
            return;

        if (layers.Count > 2)
        {
            //draw inner layers
            for (int layerIndex = 1; layerIndex < layers.Count - 1; layerIndex++)
            {
                var y = GetNodeYCoord(layerIndex, layers.Count, NodeVizer.NodeRadius);
                for (int nodeIndex = 0; nodeIndex < layers[layerIndex].Length; nodeIndex++)
                {
                    var center = new Vector2(
                        GetNodeXCoord(nodeIndex, layers[layerIndex].Length, NodeVizer.NodeRadius, maxNodesInLayer),
                        y);
                    NodeVizer.Draw(
                        layers[layerIndex][nodeIndex],
                        center);
                }
            }
        }

        //draw output nodes
        for (int nodeIndex = 0; nodeIndex < layers[^1].Length; nodeIndex++)
        {
            OutputNodeVizer.Draw(
                layers[^1][nodeIndex],
                new Vector2(
                    GetNodeXCoord(nodeIndex, layers[^1].Length, OutputNodeVizer.NodeRadius, maxNodesInLayer),
                    GetNodeYCoord(layers.Count - 1, layers.Count, OutputNodeVizer.NodeRadius)));
        }
    }

    private void DrawConnections()
    {
        var point = _mb.Points();
        if (point.Length != 0)
            Canvas.DrawMultiline(point, ConnectionVOptions.Line.Color);
    }

    internal void AppendConnections<TNode>(IList<TNode[]> layers, int maxNodesInLayer)
    {
        _mb.Clear()
            .SetPen((int)ConnectionVOptions.Line.LineType);

        for (int layerIndex = 1; layerIndex < layers.Count; layerIndex++)
        {
            var sourceLayer = layers[layerIndex - 1];
            var y = GetNodeYCoord(layerIndex, layers.Count, NodeVizer.NodeRadius);
            var ySource = GetNodeYCoord(layerIndex - 1, layers.Count, NodeVizer.NodeRadius);
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].Length; nodeIndex++)
            {
                var center = new Vector2(
                    GetNodeXCoord(nodeIndex, layers[layerIndex].Length, NodeVizer.NodeRadius, maxNodesInLayer),
                    y);
                var input = center + Vector2.Up * NodeVizer.NodeRadius;
                for (int sourceNodeIndex = 0; sourceNodeIndex < sourceLayer.Length; sourceNodeIndex++)
                {
                    var sourceCenter = new Vector2(
                        GetNodeXCoord(sourceNodeIndex, sourceLayer.Length, NodeVizer.NodeRadius, maxNodesInLayer),
                        ySource);
                    var sourceOutput = sourceCenter + Vector2.Down * NodeVizer.NodeRadius;
                    _mb.AppendLine(sourceOutput, input);
                }
            }
        }
    }

    public float GetOptimalXSpan(int nodeCount) => Size.X / (nodeCount * 2 - 1f);

    public float GetOptimalYSpan(int layerCount) => Size.Y / (layerCount * 2 - 1f);

    public float GetNodeRadius(int layerCount, int nodeCount) =>
        Mathf.Min(GetOptimalXSpan(nodeCount), GetOptimalYSpan(layerCount)) / 2f;

    public float GetNodeXCoord(int nodeIndex, int nodeCount, float nodeRadius, int maxNodesInLayer)
    {
        if (nodeCount == maxNodesInLayer)
        {
            if (nodeIndex == 0)
                return Position.X + nodeRadius;
            else if (nodeIndex == nodeCount - 1)
                return Position.X + Size.X - nodeRadius;
            else
                return Position.X + nodeRadius + (Size.X - 2 * nodeRadius) / (nodeCount - 1) * nodeIndex;
        }

        return Position.X + Size.X / nodeCount * (nodeIndex + 0.5f);
    }

    public float GetNodeYCoord(int layerIndex, int layerCount, float nodeRadius)
    {
        if (layerIndex == 0)
            return Position.Y + nodeRadius;
        else if (layerIndex < layerCount - 1)
            return Position.Y + nodeRadius + (Size.Y - 2 * nodeRadius) / (layerCount - 1) * layerIndex;
        else // last layer
            return Position.Y + Size.Y - nodeRadius;
    }
}