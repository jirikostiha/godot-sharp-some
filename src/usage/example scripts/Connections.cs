using System.Collections;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Connections : ExampleNodeBase
{
    private float _time;

    protected override void NextState(float delta)
    {
        _time += 0.1f;
    }

    public override void _Draw()
    {
        // I
        DrawSingleConnection(
            LeftMiddle(1) + new Vector2(15, 0), 8f,
            RightMiddle(1) + new Vector2(-15, 0), 12f);

        // II
        DrawTriangleConnection(
            LeftBottom(2) + new Vector2(15, 16), 8f,
            RightBottom(2) + new Vector2(-15, 16), 8f,
            MiddleTop(2) + new Vector2(0, -16), 8f);

        /// III
        DrawChain(Middle(3), 10, 12, 15, 8, 13);
    }

    private float _xOffset = 60;
    private void DrawChain(Vector2 v1, float r1, float r2, float r3, float r4, float r5)
    {
        var v2 = v1 + new Vector2(_xOffset, Sin(_time) * 20);
        var v3 = v1 + new Vector2(2 * _xOffset, Sin(_time) * 28);
        var v4 = v1 + new Vector2(3 * _xOffset, Sin(_time) * 15);
        var v5 = v1 + new Vector2(4 * _xOffset, 0);

        this.DrawCircle(v1, r1, LineColor, AreaColor);
        this.DrawCircleOutline(v2, r2, LineColor);
        this.DrawCircleOutline(v3, r3, LineColor);
        this.DrawCircleOutline(v4, r4, LineColor);
        this.DrawCircle(v5, r5, LineColor, AreaColor);

        //connections
        var points = new List<Vector2>();
        Multiline.AppendLine(points, v1, r1, v2, r2);
        Multiline.AppendLine(points, v2, r2, v3, r3);
        Multiline.AppendLine(points, v3, r3, v4, r4);
        Multiline.AppendLine(points, v4, r4, v5, r5);
        DrawMultiline(points.ToArray(), LineColor);
    }

    private void DrawTriangleConnection(Vector2 a, float ar, Vector2 b, float br, Vector2 c, float cr)
    {
        this.DrawCircleOutline(a, ar, LineColor);
        this.DrawCircleOutline(b, br, LineColor);
        this.DrawCircleOutline(c, cr, LineColor);

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
        this.DrawCircleOutline(a, ar, LineColor);
        this.DrawCircleOutline(b, br, LineColor);

        var points = new List<Vector2>(2 * 2);
        Multiline.AppendLine(points, a, ar, b, br);
        DrawMultiline(points.ToArray(), LineColor);
    }
}
