using SwimAnalytics.Application.DTOs;

namespace SwimAnalytics.Application.Services;

public interface IAnalyticsService
{
   
    Task<(PointsDto? Value, string? Error)> GetPointsAsync(int resultId, CancellationToken cancellationToken);
    Task<(PacingDto? Value, string? Error)> GetPacingAsync(int resultId, CancellationToken cancellationToken);
}