namespace GodotSharpSome.Charts2D
{
    using System;
    using Godot;
    using GodotSharpSome.Drawing2D;

    public record CoordinateSystemOptions
    {
        public AxisOptions XAxis { get; set; } = new();

        public AxisOptions YAxis { get; set; } = new();
    }

    public record AxisOptions
    {
        public string Name { get; set; } = string.Empty;

        public string? Units { get; set; }

        /// <summary> Max value on axis [units] </summary>
        public float Max { get; set; } = 1;

        /// <summary> Min value on axis [units] </summary>
        public float Min { get; set; } = 0;

        public bool ShowUnitMarkers { get; set; } = false;

        public LineOptions AxisLine { get; set; } = new ();

        //public LineOptions? MajorLines { get; set; }

        //public LineOptions? MinorLines { get; set; }
    }

    public record LineOptions
    {
        public Color? Color { get; set; }

        public LineType Type { get; set; } = LineType.Solid;

        public int Width { get; set; } = 1;
    }
}
