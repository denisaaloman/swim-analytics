using SwimAnalytics.Application.DTOs;

namespace SwimAnalytics.Application.Services;

public interface IClubService
{
    Task<PagedResult<ClubDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken cancellationToken);
    Task<ClubDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ClubDto> CreateAsync(CreateClubRequest request, CancellationToken cancellationToken);
}