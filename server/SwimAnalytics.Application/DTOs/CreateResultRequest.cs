using System.ComponentModel.DataAnnotations;

namespace SwimAnalytics.Application.DTOs;

public record CreateSplitRequest(
    [property: Range(1, int.MaxValue)] int DistanceMeters,
    [property: Range(1, int.MaxValue)] int CumulativeTimeMs);

public record CreateResultRequest(
    [property: Range(1, int.MaxValue)] int SwimmerId,
    [property: Range(1, int.MaxValue)] int EventId,
    [property: Range(1, int.MaxValue)] int MeetId,
    [property: Range(1, int.MaxValue)] int TimeMs,
    List<CreateSplitRequest>? Splits);