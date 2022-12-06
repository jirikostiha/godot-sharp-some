using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Lines : ExampleNodeBase
{
    private const float Step = 0.5f;

    private (Vector2 start, Vector2 end, Vector2 target, Vector2 current) _line;

    public Lines()
    {
        _line = (LeftMiddle(1), RightMiddle(1), RightMiddle(1), RightMiddle(1));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!Animate)
            return;

        if (_line.current.DistanceTo(_line.target) > Step)
        {
            _line.current += _line.current.DirectionTo(_line.target) * Step;
        }
        else
        {
            _line.target = (_line.target == _line.start)
                ? _line.target = _line.end
                : _line.target = _line.start;
        }

        _line.current += _line.current * _line.current.DirectionTo(_line.target) * Step;

        Update();
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Line(_line.start, _line.current),
            LineColor);

        DrawMultiline(
            Multiline.ParallelLines(_line.start, _line.current, RowHeight / 2f, 1),
            LineColor);

        DrawMultiline(
            Multiline.ParallelLine(_line.start, _line.current, -(RowHeight / 2f), CellWidth / 2f),
            LineColor);
        
        if (_line.start.DistanceTo(_line.current) > (_line.start.DistanceTo(_line.end)))
            DrawMultiline(
                Multiline.PerpendicularLine(_line.current, _line.start, RowHeight / 2f),
                LineColor);
    }

    public void _on_Animate_pressed() => Animate = !Animate;
}
