using System.ComponentModel.DataAnnotations;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record CreateMeetRequest(
    [property: Required, MaxLength(150)] string Name,
    [property: MaxLength(100)] string? City,
    DateOnly Date,
    [property: EnumDataType(typeof(PoolLength))] PoolLength PoolLength);