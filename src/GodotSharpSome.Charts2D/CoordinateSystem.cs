namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using static Godot.Mathf;

    public class CoordinateSystem : Control //colorRect
    {
        private CoordinateSystemOptions _options;
        private Vector2 _originOffset = new Vector2(20, 20);

        public CoordinateSystem()
        {
            _options = new CoordinateSystemOptions();
        }

        public CoordinateSystem(CoordinateSystemOptions options)
        {
            _options = options;
        }

        public Vector2 Origin => new Vector2(_originOffset.x, RectSize.y - _originOffset.y);

        [Export] public string XName { get => _options.XAxis.Name ?? string.Empty; set => _options.XAxis.Name = value; }
        [Export] public string XUnits { get => _options.XAxis.Units ?? string.Empty; set => _options.XAxis.Units = value; }
        [Export] public float XMax { get => _options.XAxis.Max; set => _options.XAxis.Max = value; }
        [Export] public float XMin { get => _options.XAxis.Min; set => _options.XAxis.Min = value; }
        [Export] public bool ShowXMarkers { get => _options.XAxis.ShowUnitMarkers; set => _options.XAxis.ShowUnitMarkers = value; }
        [Export] public Color XColor { get => _options.XAxis.AxisLine.Color ?? Color.ColorN("black"); set => _options.XAxis.AxisLine.Color = value; }

        [Export] public string YName { get => _options.YAxis.Name ?? string.Empty; set => _options.YAxis.Name = value; }
        [Export] public string YUnits { get => _options.YAxis.Units ?? string.Empty; set => _options.YAxis.Units = value; }
        [Export] public float YMax { get => _options.YAxis.Max; set => _options.YAxis.Max = value; }
        [Export] public float YMin { get => _options.YAxis.Min; set => _options.YAxis.Min = value; }
        [Export] public bool ShowYMarkers { get => _options.YAxis.ShowUnitMarkers; set => _options.YAxis.ShowUnitMarkers = value; }
        [Export] public Color YColor { get => _options.YAxis.AxisLine.Color ?? Color.ColorN("black"); set => _options.YAxis.AxisLine.Color = value; }

        protected string XText => string.IsNullOrEmpty(XUnits) ? XName : $"{XName} [{XUnits}]";
        protected string YText => string.IsNullOrEmpty(YUnits) ? YName : $"{YName} [{YUnits}]";

        public Vector2 ScaledSpan { get; private set; }

        /// <summary> 0 to 1 length in underlying system (control) </summary>
        public Vector2 UnitLength { get; private set; }

        protected float XUnitLength { get; private set; }
        protected float YUnitLength { get; private set; }
        protected Vector2 XAxisTop { get; private set; }
        protected Vector2 YAxisTop { get; private set; }
        protected Vector2 XScaleTop { get; private set; }
        protected Vector2 YScaleTop { get; private set; }

        protected float XMultiplier { get; private set; }

        protected float YMultiplier { get; private set; }

        public override void _Draw()
        {
            CalculateLayout();

            DrawXAxis();

            DrawYAxis();
        }

        private void CalculateLayout()
        {
            ScaledSpan = new Vector2(XMax - XMin, YMax - YMin);
            UnitLength = new Vector2(
                (RectSize.x - 10 - Origin.x - 25) / ScaledSpan.x,
                (RectSize.y - 10 - _originOffset.y - 25) / ScaledSpan.y);

            XScaleTop = Origin + Vector2.Right * ScaledSpan.x * UnitLength.x;
            YScaleTop = Origin + Vector2.Up * ScaledSpan.y * UnitLength.y;

            XAxisTop = XScaleTop + new Vector2(25, 0);
            YAxisTop = YScaleTop + new Vector2(0, -25);

            var xMaxE = CalcE(ScaledSpan.x);
            var yMaxE = CalcE(ScaledSpan.y);
        }

        private void DrawXAxis()
        {
            DrawXAxisLine();

            DrawXAxisTitle();

            DrawXAxisAuxLines();

            DrawXAxisTextValues();
        }

        private void DrawYAxis()
        {
            DrawYAxisLine();

            DrawYAxisTitle();

            DrawYAxisAuxLines();

            DrawYAxisTextValues();
        }

        public void DrawScaledPoint(Vector2 scaledPoint, Color color, float radius = 1)
        {
            var point = Map(scaledPoint);
            if (radius == 1)
                DrawMultiline(Multiline.Dot(scaledPoint), color);
            else
                this.DrawCircleArea(point, radius, color);
        }

        public void DrawScaledPoints(IEnumerable<Vector2> scaledPoints, Color color, float radius = 1)
        {
            var points = scaledPoints.Select(p => Map(p));

            if (radius == 1)
                DrawMultiline(Multiline.Dots(points), color);
            else
                foreach (var point in points)
                    this.DrawCircleArea(point, radius, color);
        }

        public void DrawText(Vector2 scaledPoint, string text, Color color, float rotationAngle = 0)
        {
            var point = Map(scaledPoint);
            DrawString(GetFont(default), point, text, color);

            //axisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
            //XText);
        }

        protected Vector2 Map(Vector2 scaledPoint) =>
            Origin
            + (scaledPoint.x - XMin) * UnitLength.x * Vector2.Right
            + (scaledPoint.y - YMin) * UnitLength.y * Vector2.Up;

        private void DrawXAxisLine()
        {
            var axis = _options.XAxis.ShowUnitMarkers
                ? Multiline.SegmentedArrow(Origin, XAxisTop, XUnitLength)
                : Multiline.Arrow(Origin, XAxisTop);

            DrawMultiline(axis, XColor, 1, true);
        }

        private void DrawYAxisLine()
        {
            var axis = _options.YAxis.ShowUnitMarkers
                ? Multiline.SegmentedArrow(Origin, YAxisTop, YUnitLength)
                : Multiline.Arrow(Origin, YAxisTop);

            DrawMultiline(axis, YColor, 1, true);
        }

        private void DrawXAxisTitle()
        {
            DrawString(GetFont(default),
                XAxisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
                XText);
        }

        private void DrawYAxisTitle()
        {
        }

        private void DrawXAxisAuxLines()
        {
            var eMax = CalcE(ScaledSpan.x * 10);
            var eStep = CalcE(ScaledSpan.x);
            var mplMax = Pow(10, eMax);
            var mplStep = Pow(10, eStep);
            var maxIntScalePoint = (int)(XMax / mplStep) * mplStep;
            var minIntScalePoint = (int)(XMin / mplStep + 1) * mplStep;


            /*
             var a = 124;
		var b = 126;
		var span = b - a;
		var eMax = CalcE(span * 10);
        var eStep = CalcE(span);
        var mplMax = Math.Pow(10, eMax);
        var mplStep = Math.Pow(10, eStep);
        var maxIntScalePoint = (int)(b / mplStep) * mplStep;
		var minIntScalePoint = (int)(a / mplStep + 1) * mplStep;
		var span2 = maxIntScalePoint - minIntScalePoint;
		var count = span2 / mplStep;
             */









            var xoffset = (XMin * XMultiplier) - (int)(XMin * XMultiplier);
            var x = (int)(xoffset * XMultiplier);
            GD.Print(xoffset);
            while (x < ScaledSpan.x)
            {
                x += 1;
                GD.Print(x);
            }

        }

        private void DrawYAxisAuxLines()
        {
        }

        private void DrawXAxisTextValues()
        {
        }

        private void DrawYAxisTextValues()
        {
        }

        private double CalcE(float span)
        {
            var e = Math.Log10(Abs(span));
            return e >= 0 ? Math.Floor(e) : Math.Floor(e);
        }

        private int CalcCount(float min, float max, out float step)
        {
            var span = max - min;
            var eMax = (int)CalcE(span * 10);
            var eStep = (int)CalcE(span);
            var mplMax = Math.Pow(10, eMax);
            var mplStep = Math.Pow(10, eStep);
            //Console.WriteLine($"mplStep: {mplStep}");
            var span10 = span / mplStep;
            if (span10 <= 3)
                mplStep /= 5;
            
            var maxIntScalePoint = (int)(max / mplStep) * mplStep;
            var minIntScalePoint = (int)(min / mplStep + 1) * mplStep;
            var span2 = maxIntScalePoint - minIntScalePoint;
            var span2i = ((int)(max / mplStep) - (int)(min / mplStep + 1)) * mplStep;
            var count = span2i / mplStep;

            step = (float)mplStep;
            return (int)count;
        }
    }
}
