using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Application.Services;

public class MeetService(AppDbContext db) : IMeetService
{
    private static readonly Expression<Func<Meet, MeetDto>> ToDto = m => new MeetDto(
        EF.Property<int>(m, "Id"),
        m.Name,
        m.City,
        m.Date,
        m.PoolLength,
        m.Results.Count());

    public async Task<PagedResult<MeetDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Meets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.Date)
            .ThenBy(m => EF.Property<int>(m, "Id"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<MeetDto>(items, page, pageSize, total);
    }

    public Task<MeetDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Meets
            .AsNoTracking()
            .Where(m => EF.Property<int>(m, "Id") == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<MeetDto> CreateAsync(CreateMeetRequest request, CancellationToken cancellationToken)
    {
        var meet = new Meet
        {
            Name = request.Name.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            Date = request.Date,
            PoolLength = request.PoolLength
        };

        db.Meets.Add(meet);
        await db.SaveChangesAsync(cancellationToken);

        var id = db.Entry(meet).Property<int>("Id").CurrentValue;
        return new MeetDto(id, meet.Name, meet.City, meet.Date, meet.PoolLength, 0);
    }
}