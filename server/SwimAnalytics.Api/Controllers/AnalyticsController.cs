using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/results/{id:int}")]
public class AnalyticsController(IAnalyticsService service) : ControllerBase
{
    [HttpGet("points")]
    public async Task<ActionResult<PointsDto>> GetPoints(int id, CancellationToken cancellationToken)
    {
        var (value, error) = await service.GetPointsAsync(id, cancellationToken);
        if (value is not null) return Ok(value);
        return error is null ? NotFound() : BadRequest(error);
    }

    [HttpGet("pacing")]
    public async Task<ActionResult<PacingDto>> GetPacing(int id, CancellationToken cancellationToken)
    {
        var (value, error) = await service.GetPacingAsync(id, cancellationToken);
        if (value is not null) return Ok(value);
        return error is null ? NotFound() : BadRequest(error);
    }
}