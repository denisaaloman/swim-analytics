using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController(IResultService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ResultDto>>> GetPage(
        [FromQuery] int? swimmerId = null,
        [FromQuery] int? eventId = null,
        [FromQuery] int? meetId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPageAsync(swimmerId, eventId, meetId, page, pageSize, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ResultDto>> Create(CreateResultRequest request, CancellationToken cancellationToken)
    {
        var (result, error) = await service.CreateAsync(request, cancellationToken);
        if (error is not null) return BadRequest(error);
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<PagedResult<LeaderboardEntryDto>>> GetLeaderboard(
        [FromQuery, Required] int? eventId,
        [FromQuery, Required] PoolLength? poolLength,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetLeaderboardAsync(eventId!.Value, poolLength!.Value, page, pageSize, cancellationToken));

    [HttpGet("personal-bests")]
    public async Task<ActionResult<IReadOnlyList<PersonalBestDto>>> GetPersonalBests(
        [FromQuery, Required] int? swimmerId,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPersonalBestsAsync(swimmerId!.Value, cancellationToken));

    [HttpGet("progression")]
    public async Task<ActionResult<IReadOnlyList<ProgressionPointDto>>> GetProgression(
        [FromQuery, Required] int? swimmerId,
        [FromQuery, Required] int? eventId,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetProgressionAsync(swimmerId!.Value, eventId!.Value, cancellationToken));
}