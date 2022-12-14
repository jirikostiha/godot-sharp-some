using System;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;

public class Crosses : ExampleNodeBase
{
    protected record Shot { public Vector2 Position; public Vector2 Destination; public Color Color; }

    private List<Shot> _shots = new();

    private Queue<(Vector2 Position, Color Color)> _markers = new();

    private Vector2 _cross1, _cross2;
    private Vector2 _origin1, _origin2;
    private Vector2 _target1, _target2;
    private Color _color1 = Color.ColorN("blue");
    private Color _color2 = Color.ColorN("red");

    public Crosses()
    {
        _origin1 = LeftTop(3);
        _cross1 = _target1 = LeftMiddle(3);
        _origin2 = MiddleTop(3);
        _cross2 = _target2 = Middle(3);
    }

    protected override void NextState(float delta)
    {
        //move shots
        for (int i = _shots.Count - 1; i >= 0; i--)
        {
            var shot = _shots[i];
            shot.Position = shot.Position.LinearInterpolate(shot.Destination, 0.05f);
            if (shot.Position.DistanceTo(shot.Destination) < 0.5f)
            {
                _markers.Enqueue((shot.Position, shot.Color));
                if (_markers.Count > 4)
                    _markers.Dequeue();
                
                _shots.RemoveAt(i);
            }
        }

        if (_cross1.DistanceTo(_target1) > 0.5f)
            //move cross
            _cross1 = _cross1.LinearInterpolate(_target1, 0.25f);
        else
        {
            //fire
            var shot = new Shot() { Position = _origin1, Destination = _cross1, Color = _color1 };
            _shots.Add(shot);
            //new target
            _target1 = NextVectorBetween(LeftBottom(3), RightMiddle(3));
        }
    }

    public override void _Draw()
    {
        DrawMultiline(
            Multiline.Cross(Middle(1), radius: 15),
            LineColor);

        DrawMultiline(
            Multiline.Cross2(Middle(2), outerRadius: 15, innerRadius: 5),
            LineColor, width: 2);

        //crosses
        DrawMultiline(Multiline.Cross2(_cross1, 10, 3), _color1);
        DrawMultiline(Multiline.Cross2(_cross2, 10, 2), _color2);

        //shots
        foreach (var shot in _shots)
            //DrawMultiline(Multiline.Dot(shot.Position), shot.Color);
            this.DrawCircleArea(shot.Position, 3, shot.Color);

        //markers
        foreach (var marker in _markers)
            DrawMultiline(Multiline.Cross(marker.Position, 3), marker.Color);
    }

    public void _on_Animate_pressed() => Animate = !Animate;
}
