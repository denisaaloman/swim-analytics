using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SwimmersController(ISwimmerService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SwimmerDto>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPageAsync(page, pageSize, search, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SwimmerDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var swimmer = await service.GetByIdAsync(id, cancellationToken);
        return swimmer is null ? NotFound() : Ok(swimmer);
    }

    [HttpPost]
    public async Task<ActionResult<SwimmerDto>> Create(CreateSwimmerRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        if (created is null) return BadRequest("Specified club does not exist.");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}