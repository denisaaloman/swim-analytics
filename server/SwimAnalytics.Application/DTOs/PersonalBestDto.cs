using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record PersonalBestDto(int EventId, Stroke Stroke, int DistanceMeters, Gender Gender, PoolLength PoolLength, int BestTimeMs);