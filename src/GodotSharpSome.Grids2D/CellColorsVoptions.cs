namespace GodotSharpSome.Grids2D
{
    using System;
    using Godot;

    public record CellColorsVOptions
    {
        public Color OddCellColor { get; set; }

        public Color EvenCellColor { get; set; }
    }
}