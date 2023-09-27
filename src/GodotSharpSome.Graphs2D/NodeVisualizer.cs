namespace GodotSharpSome.Graphs2D;

using Godot;
using GodotSharpSome.Drawing2D;

public class NodeVisualizer
{
    public NodeVisualizer(Control canvas)
    {
        Canvas = canvas;
        VOptions = new();
    }

    protected Control Canvas { get; set; }

    public NodeVOptions VOptions { get; set; }

    public float NodeRadius { get; set; } = 10f;

    public void Draw<TNode>(TNode node, Vector2 center)
    {
        Canvas.DrawCircle(center, NodeRadius, VOptions.Outline.Color, VOptions.FillColor, VOptions.Outline.Width);
        Canvas.DrawString(VOptions.Font ?? ThemeDB.FallbackFont, center, node.ToString(), 0,
            HorizontalAlignment.Center, VerticalAlignment.Center, modulate: VOptions.ValueColor);
    }
}