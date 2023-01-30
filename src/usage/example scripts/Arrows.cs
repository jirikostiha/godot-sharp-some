using System.Collections.Generic;
using Godot;
using GodotSharpSome.Drawing2D;
using static Godot.Mathf;

public class Arrows : ExampleNodeBase
{
    private float _time;
    private int _iteration;
    private float _headRadius = 15;
    private float _arrowAngle = Pi * 0.1f;

    protected override void NextState(float delta)
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
            Multiline.Arrow(LeftBottom(1), RightTop(1), _headRadius, _arrowAngle),
            LineColor);

        DrawMultiline(
            Multiline.Arrow(LeftBottom(2), Middle(2), _headRadius, Pi - _arrowAngle),
            LineColor);

        DrawMultiline(
            Multiline.DoubleArrow(LeftBottom(3), RightTop(3), _headRadius, _arrowAngle),
            LineColor);

        DrawTextRotation(Middle(4), "Relax");

        DrawTextRotationCentered(Middle(5), "Enjoy");

        DrawDimensiong(Middle(6) + 15 * Vector2.Down);
    }

    private void DrawTextRotation(Vector2 center, string text)
    {
        this.DrawCircleRegion(center, 1.5f, AreaColor);
        this.DrawString(GetFont(default), center, text, _time * 2f, LineColor);
    }

    private void DrawTextRotationCentered(Vector2 center, string text)
    {
        this.DrawCircleRegion(center, 1.5f, AreaColor);
        this.DrawCenteredString(GetFont(default), center, text, _time * 2f, LineColor);
    }

    private void DrawDimensiong(Vector2 center)
    {
        var r = 17 + 5 * (1 + Sin(_time));
        var font = GetFont(default);
        var dimAngle = Sin(_time * 0.7f);

        var v1 = center + (r * Vector2.Left).Rotated(dimAngle);
        var v2 = center + (r * Vector2.Right).Rotated(dimAngle);
        var h = r + font.GetAscent();
        var hh = h + 4;

        var dirVector = v1.DirectionTo(v2);
        var normalVector = new Vector2(dirVector.y, -dirVector.x);
        var textCenter = v1 + ((v2 - v1) / 2f) + normalVector * hh;

        var dcolor = LineColor.Lightened(0.5f);
        this.DrawLine(v1, v1 + normalVector * hh, dcolor);
        this.DrawLine(v2, v2 + normalVector * hh, dcolor);
        this.DrawMultiline(
            Multiline.DoubleArrow(v1 + normalVector * h, v2 + normalVector * h, 11, 0.18f),
            dcolor);

        this.DrawCircleOutline(center, r, LineColor);

        var rtext = r < 22
            ? r.ToString("0")
            : r < 26 ? r.ToString("0.0")
            : r.ToString("0.00");

        this.DrawCenteredString(font, textCenter, rtext, dimAngle, dcolor);
    }
}
