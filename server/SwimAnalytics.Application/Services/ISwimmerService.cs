using SwimAnalytics.Application.DTOs;

namespace SwimAnalytics.Application.Services;

public interface ISwimmerService
{
    Task<PagedResult<SwimmerDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<SwimmerDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<SwimmerDto?> CreateAsync(CreateSwimmerRequest request, CancellationToken ct);
}