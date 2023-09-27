namespace GodotSharpSome.Charts2D
{
    using Godot;

    public abstract record SeriesInfo
    {
        public string? Name { get; set; }

        //units?

        public Color? Color { get; set; }
    }
}
