using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record SegmentDto(int FromMeters, int ToMeters, int TimeMs, double Per50Ms);

public record PacingDto(
    int ResultId,
    int FirstHalfMs,
    int SecondHalfMs,
    double DifferencePercent,
    PacingProfile Profile,
    IReadOnlyList<SegmentDto> Segments);