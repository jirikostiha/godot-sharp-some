namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Line visualization options.
/// </summary>
public record LineVOptions
{
    public Color Color { get; set; } = Colors.Black;

    public LineType Type { get; set; } = LineType.Solid;

    public float Width { get; set; } = 1;
}