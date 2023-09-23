namespace GodotSharpSome.Drawing2D;

using Godot;

/// <summary>
/// https://en.wikipedia.org/wiki/Technical_drawing#Assembly_drawings
/// kóta — dimension
/// </summary>
public class TechDrawingBuilder
{
    private readonly Multiline _mb;

    public TechDrawingBuilder()
    {
        //_mb = Multiline.;
    }

    public Multiline Multiline => _mb;

    //public void AppendInnerDimmension(Vector2 point1, Vector2 point2, Color lineColor, Font font, Color textColor,
    //    float dimDistance = 14, float lineWidth = 1, bool antialiased = false)
    //{
    //    canvas.DrawMultiline(
    //        Multiline.InnerDimmensionLength(point1, point2, dimDistance),
    //        lineColor, lineWidth, antialiased);

    //    var dirVector = point1.DirectionTo(point2);
    //    var normalVector = new Vector2(dirVector.y, -dirVector.x);
    //    var textCenter = point1 + ((point2 - point1) / 2f) + normalVector * (dimDistance + 4);
    //    canvas.DrawString(font, textCenter, "42", textColor);
    //}
}