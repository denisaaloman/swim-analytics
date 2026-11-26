using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController(IClubService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClubDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPageAsync(page, pageSize, search, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClubDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var club = await service.GetByIdAsync(id, cancellationToken);
        return club is null ? NotFound() : Ok(club);
    }

    [HttpPost]
    public async Task<ActionResult<ClubDto>> Create(CreateClubRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}