namespace GodotSharpSome.Drawing2D;

using static Godot.Mathf;

internal static class SegmentedLineHelper
{
    /// <summary>
    /// Get length of adaptable part of repeating pattern where tail is fixed.
    /// </summary>
    /// <param name="totalLength"> Total length. </param>
    /// <param name="fixedInterval"> Length of fixed part of repeating pattern. </param>
    /// <param name="adaptingInterval"> Length of adaptable part of repeating pattern. </param>
    /// <param name="count"> Count of repetitions of the pattern. </param>
    internal static void AdaptSubinterval(float totalLength, float fixedInterval, ref float adaptingInterval, out int count)
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
    /// Get length of adaptable part of repeating pattern where tail is flexible.
    /// </summary>
    /// <param name="totalLength"> Total length. </param>
    /// <param name="fixedInterval"> Length of fixed part of repeating pattern. </param>
    /// <param name="adaptingInterval"> Length of adaptable part of repeating pattern. </param>
    /// <param name="count"> Count of repetitions of the pattern. </param>
    internal static void AdaptSubinterval2(float totalLength, float fixedInterval, ref float adaptingInterval, out int count)
    {
        float segmentLength = adaptingInterval + fixedInterval;
        float segmentCount = (totalLength - adaptingInterval) / segmentLength;
        count = RoundToInt(segmentCount);
        float adaptedSegmentLength = (totalLength + fixedInterval) / (count + 1);
        adaptingInterval = adaptedSegmentLength - fixedInterval;
    }
}