using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record EventDto(int Id, Stroke Stroke, int DistanceMeters, Gender Gender);