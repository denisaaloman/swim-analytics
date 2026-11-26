using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Application.Services;


public class SwimmerService(AppDbContext db) : ISwimmerService
{
    private static readonly Expression<Func<Swimmer, SwimmerDto>> ToDto = s => new SwimmerDto(
        EF.Property<int>(s, "Id"),
        s.FirstName,
        s.LastName,
        s.BirthDate,
        s.Gender,
        s.Club != null ? s.Club.Name : null);

    public async Task<PagedResult<SwimmerDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Swimmers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.LastName.Contains(search) || s.FirstName.Contains(search));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => EF.Property<int>(s, "Id"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(ct);

        return new PagedResult<SwimmerDto>(items, page, pageSize, total);
    }

    public Task<SwimmerDto?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Swimmers
            .AsNoTracking()
            .Where(s => EF.Property<int>(s, "Id") == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(ct);

    public async Task<SwimmerDto?> CreateAsync(CreateSwimmerRequest request, CancellationToken ct)
    {
        Club? club = null;
        if (request.ClubId is int clubId)
        {
            club = await db.Clubs.FirstOrDefaultAsync(c => EF.Property<int>(c, "Id") == clubId, ct);
            if (club is null) return null;
        }

        var swimmer = new Swimmer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            Club = club
        };

        db.Swimmers.Add(swimmer);
        await db.SaveChangesAsync(ct);

        var id = db.Entry(swimmer).Property<int>("Id").CurrentValue;
        return new SwimmerDto(id, swimmer.FirstName, swimmer.LastName, swimmer.BirthDate, swimmer.Gender, club?.Name);
    }
}