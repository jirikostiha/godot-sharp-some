namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using static Godot.Mathf;

    public class LineChart : CoordinateSystem
    {
        public LineChart()
        { }

        public LineChart(CoordinateSystemOptions options)
            : base(options)
        {}

        public List<Series> Series { get; set; } = new List<Series>();

        public override void _Draw()
        {
            base._Draw();

            DrawSeries();
        }

        private void DrawSeries()
        {
            foreach (var singleSeries in Series.Where(s => s is not null))
            {
                DrawScaledPoints(singleSeries.ScaledPoints, singleSeries.Color ?? Color.ColorN("red"));
                //draw connections
            }
        }
    }
}
