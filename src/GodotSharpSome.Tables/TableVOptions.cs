namespace GodotSharpSome.Tables
{
    using Godot;
    using GodotSharpSome.Drawing2D;
    using GodotSharpSome.Grids2D;

    public record TableVOptions
    {
        public OrthogonalGridVOptions GridVOptions { get; set; }

        public LineVoptions? HeaderLine { get; set; }

        public Font? HeaderFont { get; set; }

        public LineVoptions? BodyLine { get; set; }

        public Font? BodyFont { get; set; }

        public int TextMargin { get; set; }
    }
}
