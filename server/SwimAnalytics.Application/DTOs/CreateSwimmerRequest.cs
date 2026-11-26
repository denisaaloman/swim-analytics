using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.DTOs;

public record CreateSwimmerRequest(string FirstName, string LastName, DateOnly BirthDate, Gender Gender, int? ClubId);