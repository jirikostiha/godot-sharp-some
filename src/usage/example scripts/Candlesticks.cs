using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Candlesticks : ExampleNodeBase
{
    //private float _lowY, _size;
    /private float _size;
    private float _open = 20;
    private float _openDelta = 1f;
    private float _close = 40;
    private float _closeDelta = 1.2f;

    public Candlesticks()
    {
        //_lowY = Bottom();
        _size = Top() - Bottom();
    }

    protected override void NextState()
    {
        _openDelta = NextDelta(_open, _openDelta);
        _closeDelta = NextDelta(_close, _closeDelta);
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Candlestick(MiddleTop(1), 20, MiddleBottom(1), 10, 5),
            LineColor);

        DrawMultiline(
            Multiline.CrossedCandlestick(LeftTop(2), 20, LeftBottom(2), 10, 5, true),
            LineColor);

        DrawMultiline(
            Multiline.CrossedCandlestick(
                MiddleTop(2), 
                Min(_open, _close),
                MiddleBottom(2), 
                Max(_open, _close), 
                5, 
                _open < _close),
            LineColor);

        this.DrawCandlestick(
            LeftTop(3), 
            Min(_open, _close), LeftBottom(3), Max(_open, _close), 5, LineColor, 
            _open < _close ? Color.ColorN("green") : Color.ColorN("red"));
    }

    private float NextDelta(float current, float delta)
    {
        if (current + delta > _size)
             return -delta; //switch direction
        if (current + delta < 0)
            return -delta; //switch direction

        return delta;
    }
}
