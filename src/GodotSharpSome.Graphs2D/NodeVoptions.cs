namespace GodotSharpSome.Graphs2D;

using GodotSharpSome.Drawing2D;
using Godot;

public record NodeVOptions
{
    public LineVOptions Outline { get; set; } = new();

    public float MaxNodeRadius { get; set; } = 25;

    public Color FillColor { get; set; }

    public Font? Font { get; set; }

    public Color ValueColor { get; set; } = Colors.Black;
}