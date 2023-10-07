using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

namespace GodotSharpSome.Examples;

public partial class Arrows : ExampleNodeBase
{
    private double _time;
    private int _iteration;
    private float _headRadius = 15;
    private float _arrowAngle = Pi * 0.1f;

    protected override void NextState(double delta)
    {
        _time += delta;
        _iteration++;

        if (_iteration % 20 == 0)
        {
            _headRadius = NextFloat(4, 25);
            _arrowAngle = NextFloat(Pi * 0.04f, Pi * 0.25f);
        }
    }

    public override void _Draw()
    {
        DrawMultiline(
            new Multiline().AppendArrow(LeftBottom(1), RightTop(1), _headRadius, _arrowAngle).Points(),
            LineColor);

        DrawMultiline(
            new Multiline().AppendArrow(LeftBottom(2), Middle(2), _headRadius, Pi - _arrowAngle).Points(),
            LineColor);

        DrawMultiline(
            new Multiline().AppendDoubleArrow(LeftBottom(3), RightTop(3), _headRadius, _arrowAngle).Points(),
            LineColor);

        DrawTextRotation(Middle(4), "Relax");

        DrawTextRotationCentered(Middle(5), "Enjoy");

        DrawDimension(Middle(6) + 15 * Vector2.Down);
    }

    private void DrawTextRotation(Vector2 center, string text)
    {
        this.DrawCircleRegion(center, 1.5f, Colors.Red);
        this.DrawString(Font, center, text, (float)_time * 2f, modulate: LineColor);
    }

    private void DrawTextRotationCentered(Vector2 center, string text)
    {
        this.DrawCircleRegion(center, 1.5f, Colors.Red);
        this.DrawString(Font, center, text, (float)_time * 2f,
            verticalAlignment: VerticalAlignment.Center,
            textBoxHorizontalAlignment: HorizontalAlignment.Center,
            modulate: LineColor);
    }

    private void DrawDimension(Vector2 center)
    {
        var r = 17 + 5 * (1 + Sin((float)_time));
        var dimAngle = Sin((float)_time * 0.7f);

        var v1 = center + (r * Vector2.Left).Rotated(dimAngle);
        var v2 = center + (r * Vector2.Right).Rotated(dimAngle);
        var h = r + Font.GetAscent();
        var hh = h + 4;

        var dirVector = v1.DirectionTo(v2);
        var normalVector = new Vector2(dirVector.Y, -dirVector.X);
        var textCenter = v1 + ((v2 - v1) / 2f) + normalVector * hh;

        var dcolor = LineColor.Lightened(0.5f);
        this.DrawLine(v1, v1 + normalVector * hh, dcolor);
        this.DrawLine(v2, v2 + normalVector * hh, dcolor);
        this.DrawMultiline(
            new Multiline().AppendDoubleArrow(v1 + normalVector * h, v2 + normalVector * h, 11, 0.18f).Points(),
            dcolor);

        this.DrawCircleOutline(center, r, LineColor);

        var rtext = r < 22
            ? r.ToString("0")
            : r < 26
                ? r.ToString("0.0")
                : r.ToString("0.00");

        this.DrawString(Font, textCenter, rtext, dimAngle, modulate: dcolor);
    }
}