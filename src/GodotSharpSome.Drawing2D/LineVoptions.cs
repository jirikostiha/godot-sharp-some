using System.Diagnostics;

namespace GodotSharpSome.Drawing2D;

/// <summary>
/// Line visualization options.
/// </summary>
[DebuggerDisplay("{LineType}, {Color}, {Width}")]
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

    /// <summary>
    /// Color of the line.
    /// </summary>
    public Color Color { get; set; } = Colors.Gray;

    /// <summary>
    /// Type of the line.
    /// </summary>
    public LineType LineType { get; set; } = LineType.Solid;

    /// <summary>
    /// Wight of the line.
    /// </summary>
    public float Width { get; set; } = 1;
}