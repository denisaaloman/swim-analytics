using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record SplitDto(int DistanceMeters, int CumulativeTimeMs);

public record ResultDto(
    int Id,
    int SwimmerId,
    string SwimmerName,
    int EventId,
    Stroke Stroke,
    int DistanceMeters,
    Gender Gender,
    int MeetId,
    string MeetName,
    DateOnly MeetDate,
    PoolLength PoolLength,
    int TimeMs,
    IReadOnlyList<SplitDto> Splits);