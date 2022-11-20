using Godot;
using GodotSharpSome.Drawing2D;

public class Candlesticks : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Candlestick(MiddleTop(1), 20, MiddleBottom(1), 10, 5),
            LineColor);

        DrawMultiline(
            Multiline.CrossedCandlestick(LeftTop(2), 20, LeftBottom(2), 10, 5, true),
            LineColor);
        DrawMultiline(
            Multiline.CrossedCandlestick(MiddleTop(2), 20, MiddleBottom(2), 10, 5, false),
            LineColor);

        this.DrawCandlestick(LeftTop(3), 20, LeftBottom(3), 10, 5, LineColor, Color.ColorN("green"));
    }
}
