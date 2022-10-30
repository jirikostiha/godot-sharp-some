using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Drawing2D;

public class Connections : ExampleNodeBase
{
    public override void _Draw()
    {
        DrawSingleConnection(
            LeftMiddle(1) + new Vector2(15, 0), 8f,
            RightMiddle(1) + new Vector2(-15, 0), 12f);

        DrawTriangleConnection(
            LeftBottom(2) + new Vector2(15, 16), 8f,
            RightBottom(2) + new Vector2(-15, 16), 8f,
            TopMiddle(2) + new Vector2(0, -16), 8f);

        DrawNetwork();
    }

    void DrawTriangleConnection(Vector2 a, float ar, Vector2 b, float br, Vector2 c, float cr)
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

    void DrawSingleConnection(Vector2 a, float ar, Vector2 b, float br)
    {
        this.DrawCircleLine(a, ar, LineColor);
        this.DrawCircleLine(b, br, LineColor);

        var points = new List<Vector2>(2 * 2);
        Multiline.AppendLine(points, a, ar, b, br);
        DrawMultiline(points.ToArray(), LineColor);
    }

    public void DrawNetwork()
    {
        var net = new List<List<NNode>>();
        net.Add(new List<NNode>() { new NNode(), new NNode(), new NNode(), new NNode(), new NNode() });
        net.Add(new List<NNode>() { new NNode(), new NNode(), new NNode(), new NNode(), new NNode() });
        net.Add(new List<NNode>() { new NNode(), new NNode(), new NNode() });

        DrawNetwork(net, x => x.Value, new Vector2(40, 40), 20, 30);
    }

    public void DrawNetwork(List<List<NNode>> net, Func<NNode, float> sizeSelector, Vector2 first, float xDist, float yDist)
    {
        var points = new List<Vector2>(127);

        //inputs
        for (int i = 0; i < net[0].Count; i++)
        {
            var center = first + new Vector2(xDist * i, yDist);
            //Multiline.DrawCircle(points, center);
            Multiline.AppendLine(points,
                center, sizeSelector(net[0][i]),
                center + new Vector2(0, -yDist), 0);
        }

        //inner layers
        //todo


        DrawMultiline(points.ToArray(), LineColor);
    }

    public class NNode
    {
        public NNode(float value = 0)
        {
            Value = value;
        }

        public float Value { get; set; }
    }
}
