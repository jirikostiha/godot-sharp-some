namespace GodotSharpSome.Graphs2D;

using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;

public partial class LayeredNetwork<TNode> : ColorRect
{
    private LayeredNetworkVisualizer _vizer;

    public LayeredNetwork()
    {
        _vizer = new LayeredNetworkVisualizer(this)
        {
            Position = Vector2.Zero,
            Size = Size,
            ConnectionVOptions = new(),
        };

        _vizer.NodeVizer.VOptions = new();
        _vizer.InputNodeVizer.VOptions = new();
        _vizer.OutputNodeVizer.VOptions = new();

        Resized += SetDrawingSize;
    }

    public IList<TNode[]> Layers { get; set; } = new List<TNode[]>();

    public ConnectionVoptions ConnectionVoptions { get => _vizer.ConnectionVOptions; set => _vizer.ConnectionVOptions = value; }

    public NodeVOptions InputNodeVOptions { get => _vizer.InputNodeVizer.VOptions; set => _vizer.InputNodeVizer.VOptions = value; }
    public NodeVOptions NodeVOptions { get => _vizer.NodeVizer.VOptions; set => _vizer.NodeVizer.VOptions = value; }
    public NodeVOptions OutputNodeVOptions { get => _vizer.OutputNodeVizer.VOptions; set => _vizer.OutputNodeVizer.VOptions = value; }

    #region export
    [Export] public Color NodeFillColor { get => _vizer.NodeVizer.VOptions.FillColor; set => _vizer.InputNodeVizer.VOptions.FillColor = value; }
    [Export] public Color NodeValueColor { get => _vizer.NodeVizer.VOptions.ValueColor; set => _vizer.InputNodeVizer.VOptions.ValueColor = value; }
    [Export] public float MaxNodeRadius { get => _vizer.NodeVizer.VOptions.MaxNodeRadius; set => _vizer.InputNodeVizer.VOptions.MaxNodeRadius = value; }

    [Export] public LineType ConnectionLine { get => _vizer.ConnectionVOptions.Line.LineType; set => _vizer.ConnectionVOptions.Line.LineType = value; }
    [Export] public Color ConnectionColor { get => _vizer.ConnectionVOptions.Line.Color; set => _vizer.ConnectionVOptions.Line.Color = value; }
    [Export] public float ConnectionWidth { get => _vizer.ConnectionVOptions.Line.Width; set => _vizer.ConnectionVOptions.Line.Width = value; }
    #endregion

    public override void _Draw()
    {
        _vizer.Draw(Layers);
    }

    private void SetDrawingSize()
    {
        _vizer.Size = Size;
    }
}