using Godot;
using System.Collections.Generic;

namespace GodotSharpSome.Drawing2D.Examples;

public partial class Crosses : ExampleNodeBase
{
    private readonly Multiline _multiline1 = new();
    private readonly Multiline _multiline2 = new();
    private readonly Multiline _crossesMl = new();

    protected record Shot
    {
        public Vector2 Position { get; set; }
        public Vector2 Destination { get; set; }
        public int Type { get; set; }
    }

    private List<Shot> _shots = new();

    private Queue<(Vector2 Position, int Type)> _markers = new();

    private Vector2 _cross1, _cross2;
    private Vector2 _origin1, _origin2;
    private Vector2 _target1, _target2;
    private Color _color1 = new("blue");
    private Color _color2 = new("red");
    private int _time;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Crosses()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _origin1 = MiddleTop(3);
        _cross1 = _target1 = Middle(3);
        _origin2 = MiddleTop(4);
        _cross2 = _target2 = Middle(4);
    }

    [Export] public float MovementSpeed { get; set; } = 100;

    protected PathFollow2D PathFollow { get; set; }

    public override void _Ready()
    {
        PathFollow = GetNode<PathFollow2D>("Path2D/PathFollow2D");
    }

    protected override void NextState(double delta)
    {
        PathFollow.Progress = PathFollow.Progress + (float)(delta * MovementSpeed);

        //move shots
        for (int i = _shots.Count - 1; i >= 0; i--)
        {
            var shot = _shots[i];
            shot.Position = shot.Position.Lerp(shot.Destination, 0.05f);
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
            _cross1 = _cross1.Lerp(_target1, 0.25f);
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
            _cross2 = _cross2.Lerp(_target2, 0.1f);
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
        // I, II
        DrawCrossMovement(Middle(2) - Middle(1));

        // III
        DrawShooting();
    }

    private void DrawCrossMovement(Vector2 offset)
    {
        var pos = PathFollow.Position;
        var sizeCoef = Mathf.Abs(0.5f - PathFollow.ProgressRatio);

        // I
        DrawMultiline(
            _crossesMl.
            Clear().
            AppendCross(
                pos,
                radius: 8 + sizeCoef * 10f)
            .Points(),
            LineColor);

        // II
        DrawMultiline(
            _crossesMl
            .Clear()
            .AppendCross2(
                offset + pos,
                outerRadius: 10 + sizeCoef * 8f,
                innerRadius: 2 + sizeCoef * 4f)
            .Points(),
            LineColor, width: 2);
    }

    private void DrawShooting()
    {
        _multiline1.Clear();
        _multiline2.Clear();

        //aiming crosses
        _multiline1.AppendCross2(_cross1, 10, 3);
        _multiline2.AppendCross2(_cross2, 7, 2);

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
                    this.DrawCircleRegion(shot.Position, 3, _color1);
                    break;

                case 2:
                    _multiline2.AppendDot(shot.Position);
                    break;

                default: break;
            }

        //markers
        foreach (var marker in _markers)
        {
            switch (marker.Type)
            {
                case 1:
                    _multiline1.AppendCross(marker.Position, 3);
                    break;

                case 2:
                    _multiline2.AppendDot(marker.Position);
                    break;
            }
        }

        DrawMultiline(_multiline1.Points(), _color1);
        DrawMultiline(_multiline2.Points(), _color2);
    }
}