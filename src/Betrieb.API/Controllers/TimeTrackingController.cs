using Betrieb.Application.Features.TimeTracking.GetBalance;
using Betrieb.Application.Features.TimeTracking.GetEntries;
using Betrieb.Application.Features.TimeTracking.ToggleClock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Betrieb.API.Controllers;

[ApiController]
[Authorize]
[Route("api/time-tracking")]
public class TimeTrackingController(ISender sender) : ControllerBase
{
    [HttpPost("clock")]
    public async Task<ActionResult<ToggleClockResult>> ToggleClock(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ToggleClockCommand(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("entries")]
    public async Task<ActionResult<List<TimeEntryDto>>> GetEntries(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEntriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBalanceQuery(), cancellationToken);
        return Ok(result);
    }
}
