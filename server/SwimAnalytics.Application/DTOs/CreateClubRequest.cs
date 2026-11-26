using System.ComponentModel.DataAnnotations;

namespace SwimAnalytics.Application.DTOs;

public record CreateClubRequest(
    [property: Required, MaxLength(100)] string Name,
    [property: MaxLength(100)] string? City);