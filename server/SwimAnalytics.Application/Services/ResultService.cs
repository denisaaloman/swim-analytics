using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Data;
using SwimAnalytics.Domain.Entities;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public class ResultService(AppDbContext db) : IResultService
{
    private static readonly Expression<Func<Result, ResultDto>> ToDto = r => new ResultDto(
        EF.Property<int>(r, "Id"),
        EF.Property<int>(r, "SwimmerId"),
        r.Swimmer.FirstName + " " + r.Swimmer.LastName,
        EF.Property<int>(r, "EventId"),
        r.Event.Stroke,
        r.Event.DistanceMeters,
        r.Event.Gender,
        EF.Property<int>(r, "MeetId"),
        r.Meet.Name,
        r.Meet.Date,
        r.Meet.PoolLength,
        r.TimeMs,
        r.Splits
            .OrderBy(s => s.DistanceMeters)
            .Select(s => new SplitDto(s.DistanceMeters, s.CumulativeTimeMs))
            .ToList());

    public async Task<PagedResult<ResultDto>> GetPageAsync(
        int? swimmerId, int? eventId, int? meetId, int page, int pageSize, CancellationToken cancellationToken)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Results.AsNoTracking();

        if (swimmerId is not null)
            query = query.Where(r => EF.Property<int>(r, "SwimmerId") == swimmerId);
        if (eventId is not null)
            query = query.Where(r => EF.Property<int>(r, "EventId") == eventId);
        if (meetId is not null)
            query = query.Where(r => EF.Property<int>(r, "MeetId") == meetId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.Meet.Date)
            .ThenBy(r => EF.Property<int>(r, "Id"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<ResultDto>(items, page, pageSize, total);
    }

    public Task<ResultDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "Id") == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(ResultDto? Result, string? Error)> CreateAsync(CreateResultRequest request, CancellationToken cancellationToken)
    {
        var swimmer = await db.Swimmers.FindAsync(new object[] { request.SwimmerId }, cancellationToken);
        if (swimmer is null) return (null, "Swimmer does not exist.");

        var ev = await db.Events.FindAsync(new object[] { request.EventId }, cancellationToken);
        if (ev is null) return (null, "Event does not exist.");

        var meet = await db.Meets.FindAsync(new object[] { request.MeetId }, cancellationToken);
        if (meet is null) return (null, "Meet does not exist.");

        var splits = request.Splits ?? new List<CreateSplitRequest>();
        var error = ValidateSplits(splits, ev.DistanceMeters, request.TimeMs);
        if (error is not null) return (null, error);

        var result = new Result
        {
            TimeMs = request.TimeMs,
            Swimmer = swimmer,
            Event = ev,
            Meet = meet,
            Splits = splits
                .Select(s => new Split { DistanceMeters = s.DistanceMeters, CumulativeTimeMs = s.CumulativeTimeMs })
                .ToList()
        };

        db.Results.Add(result);
        await db.SaveChangesAsync(cancellationToken);

        var id = db.Entry(result).Property<int>("Id").CurrentValue;
        return (await GetByIdAsync(id, cancellationToken), null);
    }

    public async Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(
        int eventId, PoolLength poolLength, int page, int pageSize, CancellationToken cancellationToken)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var bestPerSwimmer = db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "EventId") == eventId && r.Meet.PoolLength == poolLength)
            .GroupBy(r => EF.Property<int>(r, "SwimmerId"))
            .Select(g => new { SwimmerId = g.Key, BestTimeMs = g.Min(r => r.TimeMs) });

        var total = await bestPerSwimmer.CountAsync(cancellationToken);

        var pageRows = await bestPerSwimmer
            .OrderBy(x => x.BestTimeMs)
            .ThenBy(x => x.SwimmerId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var ids = pageRows.Select(x => x.SwimmerId).ToList();

        var swimmers = await db.Swimmers
            .AsNoTracking()
            .Where(s => ids.Contains(EF.Property<int>(s, "Id")))
            .Select(s => new
            {
                Id = EF.Property<int>(s, "Id"),
                s.FirstName,
                s.LastName,
                ClubName = s.Club != null ? s.Club.Name : null
            })
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        var items = pageRows
            .Select((row, index) =>
            {
                var s = swimmers[row.SwimmerId];
                return new LeaderboardEntryDto(
                    (page - 1) * pageSize + index + 1,
                    row.SwimmerId,
                    $"{s.FirstName} {s.LastName}",
                    s.ClubName,
                    row.BestTimeMs);
            })
            .ToList();

        return new PagedResult<LeaderboardEntryDto>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<PersonalBestDto>> GetPersonalBestsAsync(int swimmerId, CancellationToken cancellationToken)
    {
        // un PB per probă și per tip de bazin (25m și 50m nu se compară)
        var bests = await db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "SwimmerId") == swimmerId)
            .GroupBy(r => new { EventId = EF.Property<int>(r, "EventId"), r.Meet.PoolLength })
            .Select(g => new { g.Key.EventId, g.Key.PoolLength, BestTimeMs = g.Min(r => r.TimeMs) })
            .ToListAsync(cancellationToken);

        var eventIds = bests.Select(b => b.EventId).Distinct().ToList();

        var events = await db.Events
            .AsNoTracking()
            .Where(e => eventIds.Contains(EF.Property<int>(e, "Id")))
            .Select(e => new { Id = EF.Property<int>(e, "Id"), e.Stroke, e.DistanceMeters, e.Gender })
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        return bests
            .Select(b =>
            {
                var e = events[b.EventId];
                return new PersonalBestDto(b.EventId, e.Stroke, e.DistanceMeters, e.Gender, b.PoolLength, b.BestTimeMs);
            })
            .OrderBy(p => p.Stroke)
            .ThenBy(p => p.DistanceMeters)
            .ThenBy(p => p.PoolLength)
            .ToList();
    }

    public async Task<IReadOnlyList<ProgressionPointDto>> GetProgressionAsync(int swimmerId, int eventId, CancellationToken cancellationToken) =>
        await db.Results
            .AsNoTracking()
            .Where(r => EF.Property<int>(r, "SwimmerId") == swimmerId
                     && EF.Property<int>(r, "EventId") == eventId)
            .OrderBy(r => r.Meet.Date)
            .ThenBy(r => EF.Property<int>(r, "Id"))
            .Select(r => new ProgressionPointDto(r.Meet.Date, r.Meet.Name, r.Meet.PoolLength, r.TimeMs))
            .ToListAsync(cancellationToken);

    private static string? ValidateSplits(List<CreateSplitRequest> splits, int eventDistance, int totalTimeMs)
    {
        var ordered = splits.OrderBy(s => s.DistanceMeters).ToList();

        if (ordered.Select(s => s.DistanceMeters).Distinct().Count() != ordered.Count)
            return "Split distances must be unique.";

        var previousTime = 0;
        foreach (var split in ordered)
        {
            if (split.DistanceMeters >= eventDistance)
                return "Split distance must be less than the event distance.";
            if (split.CumulativeTimeMs <= previousTime)
                return "Split times must increase with distance.";
            previousTime = split.CumulativeTimeMs;
        }

        if (previousTime >= totalTimeMs)
            return "The last split must be lower than the final time.";

        return null;
    }
}