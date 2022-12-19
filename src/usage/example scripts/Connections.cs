using System.Collections;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Connections : ExampleNodeBase
{
    private float _y = 0;
    private float _r1, _r2, _r3, _r4, _r5;
    private Vector2 _v1;

    public Connections()
    {
        _v1 = Middle(3);
        _r1 = 10;
        _r2 = 12;
        _r3 = 15;
        _r4 = 8;
        _r5 = 13;
    }

    protected override void NextState(float delta)
    {
        _y += 0.1f;
    }

    public override void _Draw()
    {
        DrawSingleConnection(
            LeftMiddle(1) + new Vector2(15, 0), 8f,
            RightMiddle(1) + new Vector2(-15, 0), 12f);

        DrawTriangleConnection(
            LeftBottom(2) + new Vector2(15, 16), 8f,
            RightBottom(2) + new Vector2(-15, 16), 8f,
            MiddleTop(2) + new Vector2(0, -16), 8f);

        DrawChain(Middle(3));
    }

    private float _xOffset = 60;
    private void DrawChain(Vector2 start)
    {
        var v2 = _v1 + new Vector2(_xOffset, Sin(_y) * 20);
        var v3 = _v1 + new Vector2(2 * _xOffset, Sin(_y) * 28);
        var v4 = _v1 + new Vector2(3 * _xOffset, Sin(_y) * 15);
        var v5 = _v1 + new Vector2(4 * _xOffset, 0);

        this.DrawCircle(_v1, _r1, LineColor, AreaColor);
        this.DrawCircleLine(v2, _r2, LineColor);
        this.DrawCircleLine(v3, _r3, LineColor);
        this.DrawCircleLine(v4, _r4, LineColor);
        this.DrawCircle(v5, _r5, LineColor, AreaColor);

        //connections
        var points = new List<Vector2>();
        Multiline.AppendLine(points, _v1, _r1, v2, _r2);
        Multiline.AppendLine(points, v2, _r2, v3, _r3);
        Multiline.AppendLine(points, v3, _r3, v4, _r4);
        Multiline.AppendLine(points, v4, _r4, v5, _r5);
        DrawMultiline(points.ToArray(), LineColor);
    }

    private void DrawTriangleConnection(Vector2 a, float ar, Vector2 b, float br, Vector2 c, float cr)
    {
        this.DrawCircleLine(a, ar, LineColor);
        this.DrawCircleLine(b, br, LineColor);
        this.DrawCircleLine(c, cr, LineColor);

        var points = new List<Vector2>();
        Multiline.AppendLine(points, a, ar, c, cr);
        Multiline.AppendLine(points, b, br, c, cr);

        Multiline.AppendLine(points, a, ar, a + new Vector2(0, -16), 0);
        Multiline.AppendLine(points, b, br, b + new Vector2(0, -16), 0);
        Multiline.AppendLine(points, c, cr, c + new Vector2(0, 16), 0);

        DrawMultiline(points.ToArray(), LineColor);
    }

    private void DrawSingleConnection(Vector2 a, float ar, Vector2 b, float br)
    {
        this.DrawCircleLine(a, ar, LineColor);
        this.DrawCircleLine(b, br, LineColor);

        var points = new List<Vector2>(2 * 2);
        Multiline.AppendLine(points, a, ar, b, br);
        DrawMultiline(points.ToArray(), LineColor);
    }
}
