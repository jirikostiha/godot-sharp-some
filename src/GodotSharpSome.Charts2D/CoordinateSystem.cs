namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using static Godot.Mathf;

    /// <summary>
    /// 
    /// Does not contain series.
    /// 
    /// cs point: coordinates of a point in coordinate system scale
    /// canvas point: recounted coordinates to canvas
    /// </summary>
    public partial class CoordinateSystem : ColorRect
    {
        private CoordinateSystemOptions _options;
        private CsVisualizer _vizer;

        public CoordinateSystem()
            : this(new CoordinateSystemOptions())
        { }

        public CoordinateSystem(CoordinateSystemOptions options)
        {
            _options = options;
            _vizer = new CsVisualizer(this);
        }

        /// <summary> List of graphical objects </summary>
        public List<object> GObjects { get; set; } = new List<object>();

        #region export
        [Export] public string XName { get => _options.XAxis.Name ?? string.Empty; set => _options.XAxis.Name = value; }
        [Export] public string XUnits { get => _options.XAxis.Units ?? string.Empty; set => _options.XAxis.Units = value; }
        [Export] public float XMax { get => _options.XAxis.Max; set => _options.XAxis.Max = value; }
        [Export] public float XMin { get => _options.XAxis.Min; set => _options.XAxis.Min = value; }
        [Export] public bool ShowXMarkers { get => _options.XAxis.ShowUnitMarkers; set => _options.XAxis.ShowUnitMarkers = value; }
        [Export] public Color XColor { get => _options.XAxis.AxisLine.Color ?? Colors.Black; set => _options.XAxis.AxisLine.Color = value; }

        [Export] public string YName { get => _options.YAxis.Name ?? string.Empty; set => _options.YAxis.Name = value; }
        [Export] public string YUnits { get => _options.YAxis.Units ?? string.Empty; set => _options.YAxis.Units = value; }
        [Export] public float YMax { get => _options.YAxis.Max; set => _options.YAxis.Max = value; }
        [Export] public float YMin { get => _options.YAxis.Min; set => _options.YAxis.Min = value; }
        [Export] public bool ShowYMarkers { get => _options.YAxis.ShowUnitMarkers; set => _options.YAxis.ShowUnitMarkers = value; }
        [Export] public Color YColor { get => _options.YAxis.AxisLine.Color ?? Colors.Black; set => _options.YAxis.AxisLine.Color = value; }
        #endregion

        protected string XText => string.IsNullOrEmpty(XUnits) ? XName : $"{XName} [{XUnits}]";
        protected string YText => string.IsNullOrEmpty(YUnits) ? YName : $"{YName} [{YUnits}]";

        public override void _Draw()
        {
            //use vizu builder

            //DrawSetTransformMatrix(Transform2D.FlipX);

            //CalculateLayout();

            _vizer
                .SetAreaMargin(20)
                .SetRange(XMin, XMax, YMin, YMax)
                .AddXAxisArrow(XColor)
                .AddYAxisArrow(YColor);

            //DrawXAxisLine();
            //DrawXAxisTitle();
            //DrawXAxisAuxLines();
            //DrawXAxisTextValues();
            //DrawYAxisLine();
            //DrawYAxisTitle();
            //DrawYAxisAuxLines();
            //DrawYAxisTextValues();

        }

        #region Coordinate System Api

        //public void DrawCsPoint(Vector2 csPoint, Color color, float radius = 1)
        //{
        //    DrawCanvasPoint(Map(csPoint), color, radius);
        //}

        //public void DrawCsPoints(IEnumerable<Vector2> csPoints, Color color, float radius = 1)
        //{
        //    foreach (var point in csPoints)
        //        DrawCsPoint(point, color, radius);
        //}

        //public void DrawCsText(Vector2 point, string text, Color color, float angle = 0)
        //{
        //    DrawCanvasText(Map(point), text, color, angle);
        //}

        #endregion

        protected void DrawCanvasPoints(IEnumerable<Vector2> canvasPoints, Color color, float radius = 1)
        {
            foreach (var canvasPoint in canvasPoints)
                DrawCanvasPoint(canvasPoint, color, radius);
        }

        protected void DrawCanvasPoint(Vector2 canvasPoint, Color color, float radius = 1)
        {
            if (radius == 1)
                DrawMultiline(Multiline.Dot(canvasPoint), color);
            else
                this.DrawCircleRegion(canvasPoint, radius, color);
        }

        protected void DrawCanvasText(Vector2 canvasPoint, string text, Color color, float angle = 0)
        {
            this.DrawString(ThemeDB.FallbackFont, canvasPoint, text, angle, color);

            //axisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
            //XText);
        }

        //private void CalculateLayout()
        //{
        //    ScaledSpan = new Vector2(XMax - XMin, YMax - YMin);
        //    UnitLength = new Vector2(
        //        (RectSize.x - 10 - Origin.x - 25) / ScaledSpan.x,
        //        (RectSize.y - 10 - _originOffset.y - 25) / ScaledSpan.y);

        //    XScaleTop = Origin + Vector2.Right * ScaledSpan.x * UnitLength.x;
        //    YScaleTop = Origin + Vector2.Up * ScaledSpan.y * UnitLength.y;

        //    XAxisTop = XScaleTop + new Vector2(25, 0);
        //    YAxisTop = YScaleTop + new Vector2(0, -25);

        //    var xMaxE = CalcE(ScaledSpan.x);
        //    var yMaxE = CalcE(ScaledSpan.y);
        //}

        //private void DrawXAxisLine()
        //{
        //    var axis = _options.XAxis.ShowUnitMarkers
        //        ? Multiline.SegmentedArrow(Origin, XAxisTop, UnitLength.x)
        //        : Multiline.Arrow(Origin, XAxisTop);

        //    DrawMultiline(axis, XColor, 1, true);
        //}

        //private void DrawYAxisLine()
        //{
        //    var axis = _options.YAxis.ShowUnitMarkers
        //        ? Multiline.SegmentedArrow(Origin, YAxisTop, UnitLength.y)
        //        : Multiline.Arrow(Origin, YAxisTop);

        //    DrawMultiline(axis, YColor, 1, true);
        //}

        //private void DrawXAxisTitle()
        //{
        //    DrawString(
        //        GetFont(default),
        //        XAxisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
        //        XText);
        //}

        //private void DrawYAxisTitle()
        //{
        //    this.DrawString(
        //        GetFont(default), 
        //        YAxisTop + (GetFont(default).GetStringSize(YText) + new Vector2(5, 25)) * new Vector2(1, -1),
        //        YText,
        //        Pi / 2f);
        //}

  //      private void DrawXAxisAuxLines()
  //      {
  //          var eMax = CalcE(ScaledSpan.x * 10);
  //          var eStep = CalcE(ScaledSpan.x);
  //          var mplMax = Pow(10, (float)eMax);
  //          var mplStep = Pow(10, (float)eStep);
  //          var maxIntScalePoint = (int)(XMax / mplStep) * mplStep;
  //          var minIntScalePoint = (int)(XMin / mplStep + 1) * mplStep;

  //          /*
  //           var a = 124;
		//var b = 126;
		//var span = b - a;
		//var eMax = CalcE(span * 10);
  //      var eStep = CalcE(span);
  //      var mplMax = Math.Pow(10, eMax);
  //      var mplStep = Math.Pow(10, eStep);
  //      var maxIntScalePoint = (int)(b / mplStep) * mplStep;
		//var minIntScalePoint = (int)(a / mplStep + 1) * mplStep;
		//var span2 = maxIntScalePoint - minIntScalePoint;
		//var count = span2 / mplStep;
  //           */

  //          var xoffset = (XMin * XMultiplier) - (int)(XMin * XMultiplier);
  //          var x = (int)(xoffset * XMultiplier);
  //          //GD.Print(xoffset);
  //          while (x < ScaledSpan.x)
  //          {
  //              x += 1;
  //              //GD.Print(x);
  //          }
  //      }

        private void DrawYAxisAuxLines()
        {
        }

        private void DrawXAxisTextValues()
        {
        }

        private void DrawYAxisTextValues()
        {
        }

        //public Vector2 Map(Vector2 csPoint) =>
        //    new(Origin.x + ScaleX(csPoint.x), Origin.y - ScaleY(csPoint.y));


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
