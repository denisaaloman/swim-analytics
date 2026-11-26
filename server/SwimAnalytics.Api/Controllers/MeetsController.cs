using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetsController(IMeetService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<MeetDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPageAsync(page, pageSize, search, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MeetDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var meet = await service.GetByIdAsync(id, cancellationToken);
        return meet is null ? NotFound() : Ok(meet);
    }

    [HttpPost]
    public async Task<ActionResult<MeetDto>> Create(CreateMeetRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}