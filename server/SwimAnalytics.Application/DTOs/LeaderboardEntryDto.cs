namespace SwimAnalytics.Application.DTOs;

public record LeaderboardEntryDto(int Rank, int SwimmerId, string SwimmerName, string? ClubName, int BestTimeMs);