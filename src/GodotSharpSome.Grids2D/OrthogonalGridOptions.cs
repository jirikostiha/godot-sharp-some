﻿namespace GodotSharpSome.Grids2D
{
    /// <summary>
    /// Regular orthogonal grid options
    /// </summary>
    public record OrthogonalGridOptions
    {
        public int RowCount { get; set; } = 3;
        public int ColumnCount { get; set; } = 4;
    }

    /// <summary>
    /// Irregular orthogonal grid options
    /// </summary>
    public record IrregularGridOptions
    {
        public float[] RowSpans { get; set; }
        public float[] ColumnSpans { get; set; }
    }
}