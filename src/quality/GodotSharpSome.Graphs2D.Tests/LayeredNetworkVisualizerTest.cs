using NSubstitute;
using Xunit;
using Godot;

namespace GodotSharpSome.Graphs2D.Tests;

public class LayeredNetworkVisualizerTest
{
    [Fact]
    public void DrawNet()
    {
        //var canvasMock = Substitute.For<ColorRect>(); //how to mock control?
        var inputNodeVizerMock = Substitute.For<NodeVisualizer>(null, null);
        var nodeVizerMock = Substitute.For<NodeVisualizer>(null, null);

        var net = CreateDummyNet(2, 3, 4);
        var vizer = new LayeredNetworkVisualizer(null);
        vizer.InputNodeVizer = inputNodeVizerMock;
        vizer.NodeVizer = nodeVizerMock;

        vizer.Draw(net);

        inputNodeVizerMock.Received(2).Draw(Arg.Any<DummyNode>(), Arg.Any<Vector2>());
        nodeVizerMock.Received(7).Draw(Arg.Any<DummyNode>(), Arg.Any<Vector2>());
    }

    internal List<DummyNode[]> CreateDummyNet(params int[] layerSizes)
    {
        var net = new List<DummyNode[]>();

        foreach (var layerSize in layerSizes)
        {
            var layer = new DummyNode[layerSize];
            for (int i = 0; i < layerSize; i++)
                layer[i] = new DummyNode();

            net.Add(layer);
        }

        return net;
    }

    internal class DummyNode
    {
        internal static int IdCounter = 0;

        public DummyNode() => Id = IdCounter++;

        public int Id { get; private set; }

        public override string ToString() => Id.ToString();
    }
}