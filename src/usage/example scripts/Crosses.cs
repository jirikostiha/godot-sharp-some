using System;
using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;

public class Crosses : ExampleNodeBase
{
    protected record Shot { 
        public Vector2 Position; 
        public Vector2 Destination; 
        public int Type; }

    private List<Shot> _shots = new();

    private Queue<(Vector2 Position, int Type)> _markers = new();

    private Vector2 _cross1, _cross2;
    private Vector2 _origin1, _origin2;
    private Vector2 _target1, _target2;
    private Color _color1 = Color.ColorN("blue");
    private Color _color2 = Color.ColorN("red");
    private int _time;

    public Crosses()
    {
        _origin1 = MiddleTop(3);
        _cross1 = _target1 = Middle(3);
        _origin2 = MiddleTop(4);
        _cross2 = _target2 = Middle(4);
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
                _markers.Enqueue((shot.Position, shot.Type));
                if (_markers.Count > 4)
                    _markers.Dequeue();
                
                _shots.RemoveAt(i);
            }
        }

        // Aim and fire 1
        if (_cross1.DistanceTo(_target1) > 0.5f)
            //move cross
            _cross1 = _cross1.LinearInterpolate(_target1, 0.25f);
        else
        {
            //fire
            var shot = new Shot() { Position = _origin1, Destination = _cross1, Type = 1 };
            _shots.Add(shot);
            //new target
            _target1 = NextVectorBetween(LeftBottom(3), RightMiddle(3));
        }

        // Aim 2
        if (_cross2.DistanceTo(_target2) > 0.5f)
            _cross2 =  _cross2.LinearInterpolate(_target2, 0.1f);
        else
            //new target 2
            _target2 = NextVectorBetween(LeftBottom(4), RightMiddle(4));

        // Fire 2
        if (_time % 5 == 0)
        {
            var shot = new Shot() { Position = _origin2, Destination = _cross2, Type = 2 };
            _shots.Add(shot);
        }

        _time++;
    }

    public override void _Draw()
    {
        // I
        DrawMultiline(
            Multiline.Cross(Middle(1), radius: 15),
            LineColor);

        // II
        DrawMultiline(
            Multiline.Cross2(Middle(2), outerRadius: 15, innerRadius: 5),
            LineColor, width: 2);

        // III
        DrawShooting();
    }

    private void DrawShooting()
    {
        //aiming crosses
        DrawMultiline(Multiline.Cross2(_cross1, 10, 3), _color1);
        DrawMultiline(Multiline.Cross2(_cross2, 7, 2), _color2);

        //guns
        if (_shots.Count != 0)
        {
            this.DrawRectangle(_origin1, 18, 8, _origin1.DirectionTo(_cross1).Angle(), LineColor, _color1);
            this.DrawRectangle(_origin2, 10, 4, _origin2.DirectionTo(_cross2).Angle(), LineColor, _color2);
        }

        //shots
        foreach (var shot in _shots)
            switch (shot.Type)
            {
                case 1:
                    this.DrawCircleArea(shot.Position, 3, _color1);
                    break;
                case 2:
                    DrawMultiline(Multiline.Dot(shot.Position), _color2);
                    break;


                default: break;
            }

        //markers
        foreach (var marker in _markers)
        {
            switch (marker.Type)
            {
                case 1:
                    DrawMultiline(Multiline.Cross(marker.Position, 3), _color1);
                    break;
                case 2:
                    DrawMultiline(Multiline.Dot(marker.Position), _color2);
                    break;
            }
        }
    }
}
