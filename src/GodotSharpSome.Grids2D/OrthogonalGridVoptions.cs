namespace GodotSharpSome.Grids2D
{
    using System;
    using Godot;
    using GodotSharpSome.Drawing2D;

    /// <summary>
    /// Orthogonal grid visualization options.
    /// </summary>
    public record OrthogonalGridVOptions
    {
        public LineVOptions XLine { get; set; } = new();

        public LineVOptions YLine { get; set; } = new();

        public float Overlap { get; set; }

        public CellColorsVOptions? OddRows { get; set; }

        public CellColorsVOptions? EvenRows { get; set; }

        public static OrthogonalGridVOptions CreateRowPattern(Color aColor, Color bColor = default) =>
           new()
           {
               OddRows = new()
               {
                   EvenCellColor = aColor,
                   OddCellColor = aColor
               },
               EvenRows = new()
               {
                   EvenCellColor = bColor,
                   OddCellColor = bColor
               }
           };

        public static OrthogonalGridVOptions CreateColumnPattern(Color aColor, Color bColor = default) =>
            new()
            {
                OddRows = new()
                {
                    EvenCellColor = aColor,
                    OddCellColor = bColor
                },
                EvenRows = new()
                {
                    EvenCellColor = aColor,
                    OddCellColor = bColor
                }
            };

        public static OrthogonalGridVOptions CreateCheckPattern(Color aColor, Color bColor = default) =>
            new()
            {
                OddRows = new ()
                {
                    EvenCellColor = aColor,
                    OddCellColor = bColor
                },
                EvenRows = new ()
                {
                    EvenCellColor = bColor,
                    OddCellColor = aColor
                }
            };
    }
}