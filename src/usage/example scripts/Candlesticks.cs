using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Candlesticks : ExampleNodeBase
{
    private float _size;
    private float _open = 20;
    private float _openDelta = 1.6f;
    private float _close = 60;
    private float _closeDelta = 2f;

    public Candlesticks()
    {
        _size = Top() - Bottom();
        GD.Print("open:  " + _size);
    }

    protected override void NextState()
    {
        _openDelta = UpdateDelta(_open, _openDelta);
        _open += _openDelta;

        _closeDelta = UpdateDelta(_close, _closeDelta);
        _close += _closeDelta;
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Candlestick(MiddleTop(1), 20, MiddleBottom(1), 10, 5),
            LineColor);

        DrawMultiline(
            Multiline.CrossedCandlestick(
                MiddleTop(2),
                Min(_open, _close),
                MiddleBottom(2),
                Min(_size - _open, _size - _close),
                5,
                _open < _close),
            LineColor);

        this.DrawCandlestick(
            MiddleTop(3),
            Min(_open, _close),
            MiddleBottom(3),
            Min(_size - _open, _size - _close),
            5, 
            LineColor,
            _open < _close ? Color.ColorN("green") : Color.ColorN("red"));
    }

    public void _on_Animate_pressed() => Animate = !Animate;

    private float UpdateDelta(float value, float delta)
    {
        if (value > _size)
            return -delta; //switch direction
        if (value < 0)
            return -delta; //switch direction

        return delta;
    }
}
