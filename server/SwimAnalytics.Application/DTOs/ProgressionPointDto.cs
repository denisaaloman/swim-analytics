using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record ProgressionPointDto(DateOnly Date, string MeetName, PoolLength PoolLength, int TimeMs);