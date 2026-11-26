using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SwimAnalytics.Application.DTOs;
using SwimAnalytics.Application.Services;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelaysController(IRelayService service) : ControllerBase
{
    [HttpGet("medley")]
    public async Task<ActionResult<RelayDto>> GetMedley(
        [FromQuery, Required] Gender? gender,
        [FromQuery, Required] PoolLength? poolLength,
        [FromQuery] int? clubId = null,
        CancellationToken cancellationToken = default)
    {
        var relay = await service.GetMedleyRelayAsync(gender!.Value, poolLength!.Value, clubId, cancellationToken);
        return relay is null
            ? NotFound("Not enough swimmers with 100m results to form a relay.")
            : Ok(relay);
    }
}