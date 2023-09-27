namespace GodotSharpSome.Charts2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Godot;
    using GodotSharpSome.Drawing2D;
    using static Godot.Mathf;

    public class CsVisualizer : CsVisualizer<CsVisualizer>
    {
        public CsVisualizer(Control canvas) : base(canvas) {}

        //public static implicit operator Result(CsVizuBuilder builder) => builder.Build();
    }

    /// <summary>
    /// Coordinate system visualization builder. It is interface to draw cs.
    /// cs point: coordinates of a point in coordinate system scale
    /// canvas point: recounted coordinates to canvas
    /// </summary>
    public class CsVisualizer<TSelf>
        where TSelf : CsVisualizer<TSelf>
    {
        public CsVisualizer(Control canvas)
        {
            Canvas = canvas;
            //MBuilder = new Multiline();
        }

        protected Control Canvas { get; set; }

        //protected Multiline MBuilder { get; set; }

        /// <summary> Scaled (CS) size </summary>
        //public Vector2 ScaledSpan { get; private set; }

        protected float AreaMargin { get; set; }

        /// <summary> 0 to 1 length in underlying system (control) </summary>
        //on canvas
        public Vector2 UnitLength => new(
            (XCsRange.High - XCsRange.Low) / (Canvas.Size.X - 2 * AreaMargin),
            (YCsRange.High - YCsRange.Low) / (Canvas.Size.Y - 2 * AreaMargin));

        public (float Low, float High) XCsRange { get; private set; } = new (0, 1);

        public (float Low, float High) YCsRange { get; private set; } = new (0, 1);

        //bod, kde se protinaji osy v zobrazeni, nemusi byt v nule
        protected Vector2 CsCross => new (default, default);

        //bod v canvas coordinates, kde se protinaji osy v zobrazeni, nemusi byt v nule
        protected Vector2 CanvasCross => new (default, default);

        //cross vs zero vs bottom

        // cs zero in canvas coordinates
        protected Vector2 CanvasZero => new (default, default);

        protected Vector2 XYAxisBottomCanvas => new (AreaMargin, AreaMargin);
        protected Vector2 XAxisTopCanvas => new (Canvas.Size.X - AreaMargin, AreaMargin);
        protected Vector2 YAxisTopCanvas => new Vector2(AreaMargin, Canvas.Size.Y - AreaMargin);

        //protected float XMultiplier { get; private set; }

        //protected float YMultiplier { get; private set; }

        //public float ScaleX(float csValue) => (csValue - XMin) * UnitLength.x;
        //public float ScaleY(float csValue) => (csValue - YMin) * UnitLength.y;

        //aka build
        //public void Draw()         {         }

        public TSelf SetAreaMargin(float margin)
        {
            AreaMargin = margin;
            return (TSelf)this;
        }

        public TSelf SetRange(float xmin, float xmax, float ymin, float ymax)
        {
            XCsRange = new (xmin, xmax);
            YCsRange = new (ymin, ymax);

            return (TSelf)this;
        }

        public TSelf AddCsPoint(Vector2 csPoint, Color color, float radius = 1)
        {
            AddCanvasPoint(Map(csPoint), color, radius);
            return (TSelf)this;
        }

        public TSelf AddCsPoints(IEnumerable<Vector2> csPoints, Color color, float radius = 1)
        {
            foreach (var point in csPoints)
                AddCsPoint(point, color, radius);

            return (TSelf)this;
        }

        public TSelf AddCsText(Vector2 point, string text, Color color, Font font, float angle = 0)
        {
            AddCanvasText(Map(point), text, color, font, angle);
            return (TSelf)this;
        }

        public TSelf AddCanvasPoints(IEnumerable<Vector2> canvasPoints, Color color, float radius = 1)
        {
            foreach (var canvasPoint in canvasPoints)
                AddCanvasPoint(canvasPoint, color, radius);

            return (TSelf)this;
        }

        public TSelf AddCanvasPoint(Vector2 canvasPoint, Color color, float radius = 1)
        {
            if (radius == 1)
                Canvas.DrawMultiline(Multiline.Dot(canvasPoint), color);
            else
                Canvas.DrawCircleRegion(canvasPoint, radius, color);

            return (TSelf)this;
        }

        public TSelf AddCanvasText(Vector2 canvasPoint, string text, Color color, Font font, float angle = 0)
        {
            Canvas.DrawString(font, canvasPoint, text, angle, color);

            //axisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
            //XText);
            return (TSelf)this;
        }

        public TSelf AddXAxisArrow(Color color, float width = 1, bool antialized = true)
        {
            Canvas.DrawMultiline(
                Multiline.Arrow(XYAxisBottomCanvas, XAxisTopCanvas), color, width);

            return (TSelf)this;
        }

        public TSelf AddXAxisSeparators()
        {
            //Multiline.Segmentments(Origin, XAxisTop, UnitLength.x); //paralel lines
            //DrawMultiline(axis, XColor, 1, true);

            return (TSelf)this;
        }

        public TSelf AddYAxisArrow(Color color, float width = 1, bool antialized = true)
        {
            Canvas.DrawMultiline(
                Multiline.Arrow(XYAxisBottomCanvas, YAxisTopCanvas), color, width);

            return (TSelf)this;
        }

        public TSelf AddYAxisSeparators()
        {
            //Multiline.SegmentedArrow(Origin, YAxisTop, UnitLength.y);
            //DrawMultiline(axis, YColor, 1, true);

            return (TSelf)this;
        }

        //private TSelf DrawXAxisTitle()
        //{
        //    DrawString(
        //        GetFont(default),
        //        XAxisTop + (GetFont(default).GetStringSize(XText) + new Vector2(25, 5)) * new Vector2(-1, 1),
        //        XText);

        //    return (TSelf)this;
        //}

        //private TSelf DrawYAxisTitle()
        //{
        //    this.DrawString(
        //        GetFont(default),
        //        YAxisTop + (GetFont(default).GetStringSize(YText) + new Vector2(5, 25)) * new Vector2(1, -1),
        //        YText,
        //        Pi / 2f);

        //    return (TSelf)this;
        //}

  //      private TSelf DrawXAxisAuxLines()
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

        private TSelf DrawYAxisAuxLines()
        {

            return (TSelf)this;
        }

        private TSelf DrawXAxisTextValues()
        {

            return (TSelf)this;
        }

        private TSelf DrawYAxisTextValues()
        {

            return (TSelf)this;
        }


        //todo move to charts
        public static void AppendSegmentedLine(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances)
        {
            var dir = direction.Normalized();

            AppendLine(points, start, start + dir * distances.Sum());
            AppendParallelLines(points, start, dir.LeftNormal() * 3, distances); //todo replace var by offset
        }

        //todo move to charts
        public static void AppendSegmentedArrow(IList<Vector2> points, Vector2 start, Vector2 direction, IList<float> distances,
            float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var dir = direction.Normalized();
            var segmentedPartEnd = start + dir * distances.Sum();

            AppendArrow(points, start, segmentedPartEnd + dir * 2 * headRadius, headRadius, arrowAngle);
            //todo replace var by offset
            AppendParallelLinesAlong(points, start, start + dir.LeftNormal() * 3, start.DirectionTo(segmentedPartEnd), distances);
        }
        public static void AppendAxes(IList<Vector2> points, Vector2 origin, Vector2 xDirection, float xUnitLength, int xUnitCount, float yUnitLength, int yUnitCount,
        float headRadius = Arrow_HeadRadius, float arrowAngle = Arrow_HeadAngle)
        {
            var xDistances = Enumerable.Range(0, xUnitCount).Select(i => xUnitLength).ToArray();
            var yDistances = Enumerable.Range(0, yUnitCount).Select(i => yUnitLength).ToArray();

            AppendSegmentedArrow(points, origin, xDirection, xDistances, headRadius, arrowAngle);
            AppendSegmentedArrow(points, origin, xDirection.LeftNormal(), yDistances, headRadius, arrowAngle);
        }

        //todo move to charts
        public static void AppendCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth)
        {
            var dir = low.DirectionTo(high);
            var rectBottom = low + dir * lowOffset;
            var rectTop = high - dir * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
        }

        //todo move to charts
        public static void AppendCrossedCandlestick(IList<Vector2> points, Vector2 low, float lowOffset, Vector2 high, float highOffset, float halfWidth, bool upDirrection)
        {
            var dir = low.DirectionTo(high);
            var rectBottom = low + dir * lowOffset;
            var rectTop = high - dir * highOffset;
            var rectCenter = (rectBottom + rectTop) / 2;

            AppendCandlestick(points, low, high, rectBottom, rectCenter, rectTop, halfWidth);
            if (upDirrection)
            {
                AppendLine(points,
                    new Vector2(rectBottom.X - halfWidth, rectBottom.Y),
                    new Vector2(rectTop.X + halfWidth, rectTop.Y));
            }
            else
                AppendLine(points,
                    new Vector2(rectBottom.X + halfWidth, rectBottom.Y),
                    new Vector2(rectTop.X - halfWidth, rectTop.Y));
        }


        //todo move to charts
        private static void AppendCandlestick(IList<Vector2> points, Vector2 bottom, Vector2 top, Vector2 rectBottom, Vector2 rectCenter, Vector2 rectTop, float bodyHalfWidth)
        {
            AppendLine(points, bottom, rectBottom);
            AppendLine(points, top, rectTop);
            AppendRectangle(points, rectCenter, (rectCenter - rectBottom).Length(), bodyHalfWidth, bottom.DirectionTo(top).Angle());
        }

        public Vector2 Map(Vector2 csPoint) => new(ToCanvasX(csPoint.X), ToCanvasY(csPoint.Y));

        public float ToCanvasX(float csValue) => AreaMargin + (csValue - XCsRange.Low) * UnitLength.X;
        public float ToCanvasY(float csValue) => AreaMargin + (csValue - YCsRange.Low) * UnitLength.Y;


        //private void CalculateLayout()
        //{
        //    ScaledSpan = new Vector2(XMax - XMin, YMax - YMin);
        //    UnitLength = new Vector2(
        //        (Canvas.RectSize.x - 10 - Origin.x - 25) / ScaledSpan.x,
        //        (RectSize.y - 10 - Origin.y - 25) / ScaledSpan.y);

        //    XScaleTop = Origin + Vector2.Right * ScaledSpan.x * UnitLength.x;
        //    YScaleTop = Origin + Vector2.Up * ScaledSpan.y * UnitLength.y;

        //    XAxisTop = XScaleTop + new Vector2(25, 0);
        //    YAxisTop = YScaleTop + new Vector2(0, -25);

        //    var xMaxE = CalcE(ScaledSpan.x);
        //    var yMaxE = CalcE(ScaledSpan.y);
        //}

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
