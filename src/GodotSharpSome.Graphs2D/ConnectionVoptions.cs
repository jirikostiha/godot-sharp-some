namespace GodotSharpSome.Graphs2D;

using GodotSharpSome.Drawing2D;

public record ConnectionVoptions
{
    public LineVOptions Line { get; set; } = new();
}