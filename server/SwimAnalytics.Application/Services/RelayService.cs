using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public class RelayService(AppDbContext db) : IRelayService
{
    public async Task<RelayDto?> GetMedleyRelayAsync(
        Gender gender, PoolLength poolLength, int? clubId, CancellationToken cancellationToken)
    {
        var strokes = RelayOptimizer.LegOrder;

        var query = db.Results
            .AsNoTracking()
            .Where(r => r.Event.DistanceMeters == 100
                     && r.Event.Gender == gender
                     && r.Meet.PoolLength == poolLength
                     && strokes.Contains(r.Event.Stroke));

        if (clubId is not null)
            query = query.Where(r => EF.Property<int?>(r.Swimmer, "ClubId") == clubId);

       
        var best = await query
            .GroupBy(r => new { SwimmerId = EF.Property<int>(r, "SwimmerId"), r.Event.Stroke })
            .Select(g => new { g.Key.SwimmerId, g.Key.Stroke, TimeMs = g.Min(r => r.TimeMs) })
            .ToListAsync(cancellationToken);

        var plan = RelayOptimizer.Optimize(best.Select(b => (b.SwimmerId, b.Stroke, b.TimeMs)));
        if (plan is null) return null;

        var ids = plan.Legs.Select(l => l.SwimmerId).ToList();

        var names = await db.Swimmers
            .AsNoTracking()
            .Where(s => ids.Contains(EF.Property<int>(s, "Id")))
            .Select(s => new { Id = EF.Property<int>(s, "Id"), s.FirstName, s.LastName })
            .ToDictionaryAsync(s => s.Id, s => $"{s.FirstName} {s.LastName}", cancellationToken);

        var legs = plan.Legs
            .Select(l => new RelayLegDto(l.Stroke, l.SwimmerId, names[l.SwimmerId], l.TimeMs))
            .ToList();

        return new RelayDto(plan.TotalTimeMs, legs);
    }
}