namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using static Godot.Mathf;

    public class ChartVisualizer : ChartVizualizer<ChartVisualizer>
    {
        public ChartVisualizer(Control canvas) : base(canvas) { }

        //public static implicit operator Result(ChartVizuBuilder builder) => builder.Build();
    }

    /// <summary>
    ///
    ///
    /// cs point: coordinates of a point in coordinate system scale
    /// canvas point: recounted coordinates to canvas
    /// </summary>
    public class ChartVizualizer<TSelf> : CsVisualizer<TSelf>
        where TSelf : ChartVizualizer<TSelf>
    {
        public ChartVizualizer(Control canvas)
            : base(canvas)
        {}
    }
}
