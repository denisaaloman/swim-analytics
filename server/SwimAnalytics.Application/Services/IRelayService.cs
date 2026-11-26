using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public interface IRelayService
{
    Task<RelayDto?> GetMedleyRelayAsync(Gender gender, PoolLength poolLength, int? clubId, CancellationToken cancellationToken);
}