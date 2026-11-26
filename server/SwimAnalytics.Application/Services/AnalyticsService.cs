using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;

namespace SwimAnalytics.Application.Services;

public class AnalyticsService(AppDbContext db) : IAnalyticsService
{
    public async Task<(PointsDto? Value, string? Error)> GetPointsAsync(int resultId, CancellationToken cancellationToken)
    {
        var result = await db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "Id") == resultId)
            .Select(r => new
            {
                r.TimeMs,
                EventId = EF.Property<int>(r, "EventId"),
                r.Meet.PoolLength
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null) return (null, null);

        var baseTimeMs = await db.BaseTimes
            .AsNoTracking()
            .Where(b => EF.Property<int>(b, "EventId") == result.EventId && b.PoolLength == result.PoolLength)
            .Select(b => (int?)b.BaseTimeMs)
            .FirstOrDefaultAsync(cancellationToken);

        if (baseTimeMs is null)
            return (null, "No base time defined for this event and pool length.");

        var points = PointsCalculator.Calculate(baseTimeMs.Value, result.TimeMs);
        return (new PointsDto(resultId, result.TimeMs, baseTimeMs.Value, points), null);
    }

    public async Task<(PacingDto? Value, string? Error)> GetPacingAsync(int resultId, CancellationToken cancellationToken)
    {
        var result = await db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "Id") == resultId)
            .Select(r => new
            {
                r.TimeMs,
                r.Event.DistanceMeters,
                Splits = r.Splits
                    .Select(s => new { s.DistanceMeters, s.CumulativeTimeMs })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null) return (null, null);

        var analysis = PacingAnalyzer.Analyze(
            result.DistanceMeters,
            result.TimeMs,
            result.Splits.Select(s => (s.DistanceMeters, s.CumulativeTimeMs)));

        if (analysis is null)
            return (null, "This result has no splits.");

        var dto = new PacingDto(
            resultId,
            analysis.FirstHalfMs,
            analysis.SecondHalfMs,
            analysis.DifferencePercent,
            analysis.Profile,
            analysis.Segments.Select(s => new SegmentDto(s.FromMeters, s.ToMeters, s.TimeMs, Math.Round(s.Per50Ms, 0))).ToList());

        return (dto, null);
    }
}