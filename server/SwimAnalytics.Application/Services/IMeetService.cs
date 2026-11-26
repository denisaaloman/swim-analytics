using SwimAnalytics.Application.DTOs;

namespace SwimAnalytics.Application.Services;

public interface IMeetService
{
    Task<PagedResult<MeetDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken cancellationToken);
    Task<MeetDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<MeetDto> CreateAsync(CreateMeetRequest request, CancellationToken cancellationToken);
}