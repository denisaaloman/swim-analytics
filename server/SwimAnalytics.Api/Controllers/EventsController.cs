using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll(
        [FromQuery] Stroke? stroke = null,
        [FromQuery] Gender? gender = null,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetAllAsync(stroke, gender, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var ev = await service.GetByIdAsync(id, cancellationToken);
        return ev is null ? NotFound() : Ok(ev);
    }
}