namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Line visualization options.
/// </summary>
public record LineVOptions
{
    /// <summary>
    /// Preset options to black solid line of width 1.
    /// </summary>
    public static LineVOptions BlackSolid1 => new() { LineType = LineType.Solid, Width = 1, Color = Colors.Black };

    /// <summary>
    /// Preset options to white solid line of width 1.
    /// </summary>
    public static LineVOptions WhiteSolid1 => new() { LineType = LineType.Solid, Width = 1, Color = Colors.White };

    public Color Color { get; set; } = Colors.Gray;

    public LineType LineType { get; set; } = LineType.Solid;

    public float Width { get; set; } = 1;
}