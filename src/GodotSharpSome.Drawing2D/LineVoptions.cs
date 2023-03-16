namespace GodotSharpSome.Drawing2D
{
    using Godot;

    /// <summary>
    /// Line visualization options.
    /// </summary>
    public record LineVoptions
    {
        public Color Color { get; set; }

        public LineType Type { get; set; } = LineType.Solid;

        public float Width { get; set; } = 1;
    }
}
