using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetAllAsync(Stroke? stroke, Gender? gender, CancellationToken cancellationToken);
    Task<EventDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}