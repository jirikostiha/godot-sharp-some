namespace GodotSharpSome.Drawing2D;

using static Godot.Mathf;

public static class SegmentedLineHelper
{
    /// <summary>
    /// with tail - fixed
    /// </summary>
    public static void AdaptSubinterval(float totalLength, float fixedInterval, ref float adaptingInterval, out int count)
    {
        float segmentLength = adaptingInterval + fixedInterval;
        float segmentCount = totalLength / segmentLength;
        count = RoundToInt(segmentCount);
        if (count > 0)
        {
            float adaptedSegmentLength = (totalLength - fixedInterval) / count;
            adaptingInterval = adaptedSegmentLength - fixedInterval;
        }
        else
        {
            adaptingInterval = 0;
        }
    }

    /// <summary>
    /// tail - flexible
    /// </summary>
    public static void AdaptSubinterval2(float totalLength, float fixedInterval, ref float adaptingInterval, out int count)
    {
        float segmentLength = adaptingInterval + fixedInterval;
        float segmentCount = (totalLength - adaptingInterval) / segmentLength;
        count = RoundToInt(segmentCount);
        float adaptedSegmentLength = (totalLength + fixedInterval) / (count + 1);
        adaptingInterval = adaptedSegmentLength - fixedInterval;
    }
}