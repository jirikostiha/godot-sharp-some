using Godot;
using System.Collections.Generic;
using static Godot.Mathf;

namespace GodotSharpSome.Drawing2D.Examples;

public partial class Rectangles : ExampleNodeBase
{
    private const float Tolerance = 1;

    private float _length, _nextLength;
    private float _width, _nextWidth;
    private float _angle, _nextAngle;

    private Queue<(float Length, float Width, float Angle)> _tracking = new();

    public Rectangles()
    {
        _length = _nextLength = 68f;
        _width = _nextWidth = 24f;
        _angle = _nextAngle = Pi / 5.1f;
    }

    protected override void NextState(double delta)
    {
        if (_tracking.Count > 10)
            _tracking.Dequeue();

        _tracking.Enqueue((_length, _width, _angle));

        if (Abs(_nextLength - _length) < Tolerance)
            _nextLength = NextFloat(4, 80);

        if (Abs(_nextWidth - _width) < Tolerance)
            _nextWidth = NextFloat(4, 40);

        if (Abs(_nextAngle - _angle) < Tolerance)
            _nextAngle = NextFloat(0, 2 * Pi);

        _length = Lerp(_length, _nextLength, 0.1f);
        _width = Lerp(_width, _nextWidth, 0.04f);
        _angle = Lerp(_angle, _nextAngle, 0.1f);
    }

    public override void _Draw()
    {
        // I
        this.DrawRectangleOutline(Middle(1), _length, _width, _angle, LineColor);

        // II
        this.DrawRectangleRegion(Middle(2), _length, _width, _angle, AreaColor);

        // III
        this.DrawRectangle(Middle(3), _length, _width, _angle, LineColor, AreaColor);

        // I, II, III
        _DrawTracks(Middle(1), Middle(2), Middle(3));
    }

    public void _DrawTracks(Vector2 center1, Vector2 center2, Vector2 center3)
    {
        var lineColor = new Color(LineColor, 0.1f);
        var areaColor = new Color(AreaColor, 0.1f);
        foreach (var item in _tracking)
        {
            lineColor = lineColor.Lightened(1 / (float)_tracking.Count);
            this.DrawRectangleOutline(Middle(1), item.Length, item.Width, item.Angle, lineColor);
            this.DrawRectangleRegion(Middle(2), item.Length, item.Width, item.Angle, areaColor);
            this.DrawRectangle(center3, item.Length, item.Width, item.Angle, lineColor, areaColor);
        }
    }
}