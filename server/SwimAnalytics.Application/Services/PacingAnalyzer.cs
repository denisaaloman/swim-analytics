using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public record SegmentPace(int FromMeters, int ToMeters, int TimeMs, double Per50Ms);

public record PacingAnalysis(
    int FirstHalfMs,
    int SecondHalfMs,
    double DifferencePercent,
    PacingProfile Profile,
    IReadOnlyList<SegmentPace> Segments);

public static class PacingAnalyzer
{
    private const double EvenThresholdPercent = 1.0;

    public static PacingAnalysis? Analyze(
        int eventDistance,
        int totalTimeMs,
        IEnumerable<(int DistanceMeters, int CumulativeTimeMs)> splits)
    {
        var points = new List<(int Distance, int Time)> { (0, 0) };
        points.AddRange(splits
            .Where(s => s.DistanceMeters > 0 && s.DistanceMeters < eventDistance)
            .OrderBy(s => s.DistanceMeters)
            .Select(s => (s.DistanceMeters, s.CumulativeTimeMs)));

        if (points.Count == 1) return null;

        points.Add((eventDistance, totalTimeMs));

        var segments = new List<SegmentPace>();
        for (var i = 1; i < points.Count; i++)
        {
            var distance = points[i].Distance - points[i - 1].Distance;
            var time = points[i].Time - points[i - 1].Time;
            segments.Add(new SegmentPace(points[i - 1].Distance, points[i].Distance, time, time * 50.0 / distance));
        }

        var firstHalf = (int)Math.Round(TimeAt(points, eventDistance / 2.0));
        var secondHalf = totalTimeMs - firstHalf;
        var diffPercent = (secondHalf - firstHalf) * 100.0 / firstHalf;

        var profile = diffPercent > EvenThresholdPercent ? PacingProfile.PositiveSplit
                    : diffPercent < -EvenThresholdPercent ? PacingProfile.NegativeSplit
                    : PacingProfile.Even;

        return new PacingAnalysis(firstHalf, secondHalf, Math.Round(diffPercent, 2), profile, segments);
    }

    private static double TimeAt(List<(int Distance, int Time)> points, double distance)
    {
        for (var i = 1; i < points.Count; i++)
        {
            if (distance <= points[i].Distance)
            {
                var (d0, t0) = points[i - 1];
                var (d1, t1) = points[i];
                return t0 + (t1 - t0) * (distance - d0) / (d1 - d0);
            }
        }
        return points[^1].Time;
    }
}