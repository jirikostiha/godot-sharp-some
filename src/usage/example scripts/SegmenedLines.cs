using GodotSharpSome.Drawing2D;

public partial class SegmenedLines : ExampleNodeBase
{
    public override void _Draw()
    {
        // I
        DrawMultiline(
            Multiline.SegmentedLine(LeftMiddle(1), RightMiddle(1), segmentCount: 4),
            LineColor);

        // II
        DrawMultiline(
            Multiline.SegmentedArrow(LeftTop(2), RightBottom(3), segmentLength: 20, headRadius: 15),
            LineColor);
    }
}