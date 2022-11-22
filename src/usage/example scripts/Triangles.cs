using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Triangles : ExampleNodeBase
{
    private const float Tolerance = 1;

    private Vector2 _p1, _p2, _p3;
    private Vector2 _p1Next, _p2Next, _p3Next;

    public Triangles()
    {
        _p1 = _p1Next = LeftTop(1);
        _p2 = _p2Next = RightTop(1);
        _p3 = _p3Next = MiddleBottom(1);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!Animate)
            return;

        if (Abs(_p1.DistanceTo(_p1Next)) < Tolerance)
            _p1Next = NextVectorInsideCell(1);

        if (Abs(_p2.DistanceTo(_p2Next)) < Tolerance)
            _p2Next = NextVectorInsideCell(1);

        if (Abs(_p3.DistanceTo(_p3Next)) < Tolerance)
            _p3Next = NextVectorInsideCell(1);

        _p1 = _p1.Lerp(_p1Next, 0.13f);
        _p2 = _p2.Lerp(_p2Next, 0.20f);
        _p3 = _p3.Lerp(_p3Next, 0.07f);

        Update();
    }

    public override void _Draw()
    {
        this.DrawTriangleLine(_p1, _p2, _p3, LineColor);

        this.DrawTriangleArea(_p1 + CellWidthVector, _p2 + CellWidthVector, _p3 + CellWidthVector, AreaColor);

        this.DrawTriangle(_p1 + 2 * CellWidthVector, _p2 + 2 * CellWidthVector, _p3 + 2 * CellWidthVector, LineColor, AreaColor);
    }
}
