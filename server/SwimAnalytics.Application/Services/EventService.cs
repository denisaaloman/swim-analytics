using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Entities;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public class EventService(AppDbContext db) : IEventService
{
    private static readonly Expression<Func<Event, EventDto>> ToDto = e => new EventDto(
        EF.Property<int>(e, "Id"),
        e.Stroke,
        e.DistanceMeters,
        e.Gender);

    public async Task<IReadOnlyList<EventDto>> GetAllAsync(Stroke? stroke, Gender? gender, CancellationToken cancellationToken)
    {
        var query = db.Events.AsNoTracking();

        if (stroke is not null)
            query = query.Where(e => e.Stroke == stroke);

        if (gender is not null)
            query = query.Where(e => e.Gender == gender);

        return await query
            .OrderBy(e => e.Stroke)
            .ThenBy(e => e.DistanceMeters)
            .ThenBy(e => e.Gender)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    public Task<EventDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Events
            .AsNoTracking()
            .Where(e => EF.Property<int>(e, "Id") == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);
}