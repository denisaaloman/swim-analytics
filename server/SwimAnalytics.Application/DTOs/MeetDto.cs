using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record MeetDto(int Id, string Name, string? City, DateOnly Date, PoolLength PoolLength, int ResultCount);