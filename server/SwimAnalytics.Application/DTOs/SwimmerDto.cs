using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record SwimmerDto(int Id, string FirstName, string LastName, DateOnly BirthDate, Gender Gender, string? ClubName);