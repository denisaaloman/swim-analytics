using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Application.Services;

public class ClubService(AppDbContext db) : IClubService
{
    private static readonly Expression<Func<Club, ClubDto>> ToDto = c => new ClubDto(
        EF.Property<int>(c, "Id"),
        c.Name,
        c.City,
        c.Swimmers.Count());

    public async Task<PagedResult<ClubDto>> GetPageAsync(int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Clubs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .ThenBy(c => EF.Property<int>(c, "Id"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<ClubDto>(items, page, pageSize, total);
    }

    public Task<ClubDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Clubs
            .AsNoTracking()
            .Where(c => EF.Property<int>(c, "Id") == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ClubDto> CreateAsync(CreateClubRequest request, CancellationToken cancellationToken)
    {
        var club = new Club
        {
            Name = request.Name.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim()
        };

        db.Clubs.Add(club);
        await db.SaveChangesAsync(cancellationToken);

        var id = db.Entry(club).Property<int>("Id").CurrentValue;
        return new ClubDto(id, club.Name, club.City, 0);
    }
}