using Godot;
using GodotSharpSome.Drawing2D;
using System.Collections.Generic;
using System.Linq;

public partial class Vectors : ExampleNodeBase
{
    private const float DistanceInterpolationDelta = 2;

    private Multiline _multiline = new();

    private readonly List<Vector2> _vectors;
    private readonly List<Vector2> _targetVectors;

    public Vectors()
    {
        _vectors = new List<Vector2>
        {
            new Vector2(40, 40),
            new Vector2(30, 60),
            new Vector2(60, 20),
            new Vector2(40, -40),
            new Vector2(0, 30),
            new Vector2(-30, 20),
            new Vector2(-20, -50),
        };

        _targetVectors = new List<Vector2>(_vectors.Count / 2);
        for (int i = 0; i < _vectors.Count / 2; i++)
            _targetVectors.Add(_vectors[i]);
    }

    protected override void NextState(double delta)
    {
        for (int i = 0; i < _vectors.Count; i++)
        {
            if (i < _vectors.Count / 2) // position interpolations
            {
                if (_vectors[i].DistanceTo(_targetVectors[i]) <= DistanceInterpolationDelta)
                {
                    if (_vectors[i] == _targetVectors[i])
                        _targetVectors[i] = NextVectorBetween(new Vector2(-80, -80), new Vector2(80, 80));
                    else
                        _vectors[i] = _targetVectors[i];
                }
                _vectors[i] += _vectors[i].DirectionTo(_targetVectors[i]) * DistanceInterpolationDelta;
            }
            else // rotation interpolations
            {
                _vectors[i] = _vectors[i].Rotated(NextFloat(0, i % 2 == 0 ? 0.01f : -0.02f));
            }
        }
    }

    public override void _Draw()
    {
        // I
        DrawMultiline(
            _multiline.Clear().AppendVectorsAbsolutely(new Vector2(100, 200), _vectors).Points(),
            LineColor);

        // II
        DrawVectorSummation(new Vector2(300, 200));

        // III
        DrawVectorSummationOrdered(new Vector2(500, 200));
    }

    private void DrawVectorSummation(Vector2 origin)
    {
        DrawMultiline(
            _multiline.Clear().AppendVectorsRelatively(origin, _vectors).Points(),
            LineColor);

        var sumVector = _vectors.Aggregate((a, b) => a + b);
        DrawMultiline(
            _multiline.Clear().AppendArrow(origin, origin + sumVector).Points(),
            LineColor2);
    }

    private void DrawVectorSummationOrdered(Vector2 origin)
    {
        DrawMultiline(
            _multiline.Clear().AppendVectorsRelatively(origin, _vectors.OrderBy(x => x.Angle()).ToArray()).Points(),
            LineColor);

        var sumVector = _vectors.Aggregate((a, b) => a + b);
        DrawMultiline(
            _multiline.Clear().AppendArrow(origin, origin + sumVector).Points(),
            LineColor2);
    }
}