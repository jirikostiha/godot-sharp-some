using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public partial class Candlesticks : ExampleNodeBase
{
    private float _size;
    private float _open = 20;
    private float _openDelta = 1.6f;
    private float _close = 60;
    private float _closeDelta = 2f;

    public Candlesticks()
    {
        _size = Top() - Bottom();
    }

    protected override void NextState(double delta)
    {
        _openDelta = UpdateDelta(_open, _openDelta);
        _open += _openDelta;

        _closeDelta = UpdateDelta(_close, _closeDelta);
        _close += _closeDelta;
    }

    public override void _Draw()
    {
        // I
        DrawMultiline(
            Multiline.Candlestick(MiddleTop(1), 20, MiddleBottom(1), 10, 5),
            LineColor);

        // II
        DrawMultiline(
            Multiline.CrossedCandlestick(
                MiddleTop(2),
                Min(_open, _close),
                MiddleBottom(2),
                Min(_size - _open, _size - _close),
                5,
                _open < _close),
            LineColor);

        // III
        this.DrawCandlestick(
            MiddleTop(3),
            Min(_open, _close),
            MiddleBottom(3),
            Min(_size - _open, _size - _close),
            5,
            LineColor,
            _open < _close ? new Color("green") : new Color("red"));
    }

    private float UpdateDelta(float value, float delta)
    {
        if (value > _size)
            return -delta; //switch direction
        if (value < 0)
            return -delta; //switch direction

        return delta;
    }
}