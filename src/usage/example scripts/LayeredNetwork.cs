using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public partial class LayeredNetwork : ExampleNodeBase
{
    private int[] _layerSizes = new int[] { 12, 18, 13, 10 };
    private float _nodeRadius = 10;
    private List<List<int>> _layers;

    public LayeredNetwork()
    {
        var layers = new List<List<int>>();
        for (int layerIndex = 0; layerIndex < _layerSizes.Length; layerIndex++)
        {
            var layer = new List<int>();
            for (int nodeIndex = 0; nodeIndex < _layerSizes[layerIndex]; nodeIndex++)
                layer.Add(NextInt(0, 4));

            layers.Add(layer);
        }
        _layers = layers;
    }

    public override void _Draw()
    {
        var origin = LeftBottom(1) + new Vector2(_nodeRadius, _nodeRadius);
        var nodeSpan = 26;
        var layerSpan = 200;
        DrawNodes(origin, nodeSpan, layerSpan);
        DrawConnections(origin, nodeSpan, layerSpan);
    }

    public void DrawNodes(Vector2 origin, float nodeSpan, float layerSpan)
    {
        var maxNodes = _layers.Max(l => l.Count);
        for (int layerIndex = 0; layerIndex < _layers.Count; layerIndex++)
        {
            var offset = (maxNodes - _layers[layerIndex].Count) / 2f * nodeSpan;
            for (int nodeIndex = 0; nodeIndex < _layers[layerIndex].Count; nodeIndex++)
            {
                var center = origin + new Vector2(layerIndex * layerSpan, offset + nodeIndex * nodeSpan);
                if (layerIndex == 0)
                    this.DrawCircle(center, _nodeRadius, LineColor, AreaColor);
                else
                    this.DrawCircleOutline(center, _nodeRadius, LineColor);

                var value = _layers[layerIndex][nodeIndex] + NextInt(0, 2);
                DrawString(Font, center + new Vector2(-5, 5), value.ToString(), HorizontalAlignment.Left, -1, 16, TextColor);
            }
        }
    }

    public void DrawConnections(Vector2 origin, float nodeSpan, float layerSpan)
    {
        var maxNodes = _layers.Max(l => l.Count);
        var m = new Multiline(_layers.Count * maxNodes * 2);
        for (int layerIndex = 1; layerIndex < _layers.Count; layerIndex++)
        {
            var sourceLayer = _layers[layerIndex - 1];
            var destinationLayer = _layers[layerIndex];
            var offset = (maxNodes - _layers[layerIndex].Count) / 2f * nodeSpan;
            for (int destNodeIndex = 0; destNodeIndex < _layers[layerIndex].Count; destNodeIndex++)
            {
                var center = origin + new Vector2(layerIndex * layerSpan, offset + destNodeIndex * nodeSpan);
                var sourceOffset = (maxNodes - sourceLayer.Count) / 2f * nodeSpan;
                for (int sourceNodeIndex = 0; sourceNodeIndex < sourceLayer.Count; sourceNodeIndex++)
                {
                    m.AppendConnection(
                        origin + new Vector2((layerIndex - 1) * layerSpan, sourceOffset + sourceNodeIndex * nodeSpan),
                        _nodeRadius,
                        center,
                        _nodeRadius);
                }
            }
        }

        DrawMultiline(m.Points, new Color(LineColor, 0.1f));
    }
}
