namespace GodotSharpSome.Drawing2D
{
    using Godot;

    public record LineOptions
    {
        public Color? Color { get; set; }

        public LineType Type { get; set; } = LineType.Solid;

        public float Width { get; set; } = 1;
    }
}
