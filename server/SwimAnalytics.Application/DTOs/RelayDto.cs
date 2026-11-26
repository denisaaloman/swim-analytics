using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record RelayLegDto(Stroke Stroke, int SwimmerId, string SwimmerName, int TimeMs);

public record RelayDto(int TotalTimeMs, IReadOnlyList<RelayLegDto> Legs);