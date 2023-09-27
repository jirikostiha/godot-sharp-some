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
    /// Non-interactive chart drawn by multiline.
    /// </summary>
    public partial class Chart : CoordinateSystem
    {
        public Chart()
        { }

        public Chart(CoordinateSystemOptions options)
            : base(options)
        { }

        public List<SeriesInfo> Series { get; set; } = new List<SeriesInfo>();

        public override void _Draw()
        {
            base._Draw();

            //DrawCircle(Vector2.Zero, 10, Color.ColorN("orange"));
            //GD.Print("origin: " + Origin);
            //DrawCircle(UnitLength, 3, Color.ColorN("magenta"));
            //GD.Print("unit: " + UnitLength);
            //GD.Print($"xmin: {XMin}, xmax: {XMax}, ymin: {YMin}, ymax: {YMax}");

            //var cspoint = Map(Vector2.Zero);
            //GD.Print("cs[0,0]: cv" + cspoint);
            //DrawCircle(cspoint, 7, Color.ColorN("red"));
            //var cspoint2 = Map(new Vector2(1,1));
            //GD.Print("cs [1,1]: " + cspoint2);
            //DrawCircle(cspoint2, 2, Color.ColorN("red"));
            //DrawLine(Origin, cspoint2, Color.ColorN("red"));

            GD.Print("-----------");

            DrawSeries();
        }

        private void DrawSeries()
        {
            foreach (var singleSeries in Series.Where(s => s is not null))
            {
                switch (singleSeries)
                {
                    case ILineSeries lineSeries:
                        //DrawCsPoints(lineSeries.Points, singleSeries.Color ?? Color.ColorN("red"));
                        //draw connections
                        break;

                    case ICandleSeries candleSeries:
                        DrawCsCandles(candleSeries.Candles, Colors.Black, singleSeries.Color ?? Colors.LightGray);
                        break;
                }
            }
        }

        public void DrawCsCandles(
            IEnumerable<(float X, Candle Candle)> csCandles,
            Color bodyColor,
            Color lineColor,
            float lineWidth = 1,
            bool antialiased = false)
        {
            foreach (var csCandle in csCandles)
                DrawCsCandle(csCandle, default, lineColor, lineWidth, antialiased);
        }

        public void DrawCsCandle(
            (float X, Candle Candle) csCandle,
            Color bodyColor,
            Color lineColor,
            float lineWidth = 1,
            bool antialiased = false)
        {
            //GD.Print("cs: " + csCandle);
            //GD.Print("cv: " + MapCandle(csCandle));
            //DrawCanvasCandle(MapCandle(csCandle), bodyColor, lineColor, lineWidth, antialiased);
        }

        protected void DrawCanvasCandle(
            (float X, Candle Candle) canvasCandle,
            Color bodyColor,
            Color lineColor,
            float lineWidth = 1,
            bool antialiased = false)
        {
            var low = new Vector2(canvasCandle.X, canvasCandle.Candle.Low);
            var size = Abs(canvasCandle.Candle.High - canvasCandle.Candle.Low);
            var high = low + size * Vector2.Up;
            var lowoff = Min(canvasCandle.Candle.Low - canvasCandle.Candle.Open, canvasCandle.Candle.Low - canvasCandle.Candle.Close);
            var highoff = Min(canvasCandle.Candle.Open - canvasCandle.Candle.High, canvasCandle.Candle.Close - canvasCandle.Candle.High);
            //GD.Print("cv low: " + low);
            //GD.Print("cv size: " + size);
            //GD.Print("cv high: " + high);
            //GD.Print("cv lowoff: " + lowoff);
            //GD.Print("cv closeoff: " + highoff);

            //DrawLine(Origin, low, Color.ColorN("green"));
            //DrawLine(Origin, high, Color.ColorN("blue"));
            //(Origin, low + lowoff * Vector2.Up, Color.ColorN("lightgreen"));
            //this.DrawLine(Origin, open, Color.ColorN("orange"));

            DrawMultiline(
                Multiline.CrossedCandlestick(low, lowoff, high, highoff, 4, canvasCandle.Candle.Open > canvasCandle.Candle.Close),
                Colors.Red);

            //this.DrawCandlestick(
            //    low,
            //    Min(canvasCandle.Open, canvasCandle.Close),
            //    high,
            //    Min(size - canvasCandle.Open, size - canvasCandle.Close),
            //    5,
            //    lineColor,
            //    bodyColor,
            //    lineWidth,
            //    antialiased);
        }

        //public (float X, Candle Candle) ScaleCandle((float X, Candle Candle) csCandle)
        //    => ((csCandle.X - XMin) * UnitLength.x,
        //        new Candle()
        //        {
        //            Low = (csCandle.Candle.Low - YMin) * UnitLength.y,
        //            High = (csCandle.Candle.High - YMin) * UnitLength.y,
        //            Open = (csCandle.Candle.Open - YMin) * UnitLength.y,
        //            Close = (csCandle.Candle.Close - YMin) * UnitLength.y
        //        });

        /// <summary>
        /// Map candle in cs to point in canvas coords
        /// </summary>
    //    public (float X, Candle Candle) MapCandle((float X, Candle Candle) csCandle)
    //    {
    //        var scaledCandle = ScaleCandle(csCandle);
    //        return (Origin.x + UnitLength.x + scaledCandle.X,
    //            new Candle()
    //            {
    //                Low = Origin.y - scaledCandle.Candle.Low,
    //                High = Origin.y - scaledCandle.Candle.High,
    //                Open = Origin.y - scaledCandle.Candle.Open,
    //                Close = Origin.y - scaledCandle.Candle.Close
    //            });
    //    }
    }
}
