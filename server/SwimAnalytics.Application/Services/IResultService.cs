using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public interface IResultService
{
    Task<PagedResult<ResultDto>> GetPageAsync(int? swimmerId, int? eventId, int? meetId, int page, int pageSize, CancellationToken cancellationToken);
    Task<ResultDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<(ResultDto? Result, string? Error)> CreateAsync(CreateResultRequest request, CancellationToken cancellationToken);

    Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(int eventId, PoolLength poolLength, int page, int pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<PersonalBestDto>> GetPersonalBestsAsync(int swimmerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProgressionPointDto>> GetProgressionAsync(int swimmerId, int eventId, CancellationToken cancellationToken);
}