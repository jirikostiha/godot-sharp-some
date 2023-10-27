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
    /// <returns> Count of repetitions of the pattern </returns>
    internal static int AdaptSubinterval(float totalLength, float fixedInterval, ref float adaptingInterval)
    {
        float segmentLength = adaptingInterval + fixedInterval;
        float segmentCount = totalLength / segmentLength;
        var repetitionCount = RoundToInt(segmentCount);
        if (repetitionCount > 0)
        {
            float adaptedSegmentLength = (totalLength - fixedInterval) / repetitionCount;
            adaptingInterval = adaptedSegmentLength - fixedInterval;
            return repetitionCount;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Get length of adaptable part of repeating pattern where tail is flexible.
    /// </summary>
    /// <param name="totalLength"> Total length. </param>
    /// <param name="fixedInterval"> Length of fixed part of repeating pattern. </param>
    /// <param name="adaptingInterval"> Length of adaptable part of repeating pattern. </param>
    /// <returns> Count of repetitions of the pattern </returns>
    internal static int AdaptSubinterval2(float totalLength, float fixedInterval, ref float adaptingInterval)
    {
        float segmentLength = adaptingInterval + fixedInterval;
        float segmentCount = (totalLength - adaptingInterval) / segmentLength;
        var repetitionCount = RoundToInt(segmentCount);
        float adaptedSegmentLength = (totalLength + fixedInterval) / (repetitionCount + 1);
        adaptingInterval = adaptedSegmentLength - fixedInterval;

        return repetitionCount;
    }
}