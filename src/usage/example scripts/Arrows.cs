using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Arrows: ExampleNodeBase
{
    private float _time;
    private float _headRadius = 15;
    private float _arrowAngle = Pi * 0.1f;

    protected override void NextState(float delta)
    {
        _time = _time < 0.5 ? _time + delta : 0;

        if (_time == 0)
        {
            _headRadius = NextFloat(4, 25);
            _arrowAngle = NextFloat(Pi * 0.04f, Pi * 0.25f);
        }
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Arrow(LeftBottom(1), RightTop(1), _headRadius, _arrowAngle),
            LineColor);

        DrawMultiline(
            Multiline.Arrow(LeftBottom(2), Middle(2), _headRadius, Pi - _arrowAngle),
            LineColor);

        DrawMultiline(
            Multiline.DoubleArrow(LeftBottom(3), RightTop(3), _headRadius, _arrowAngle),
            LineColor);
    }
}
