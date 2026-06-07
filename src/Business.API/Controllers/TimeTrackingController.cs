using Business.Application.Features.TimeTracking.GetBalance;
using Business.Application.Features.TimeTracking.GetClockStatus;
using Business.Application.Features.TimeTracking.GetEntries;
using Business.Application.Features.TimeTracking.ToggleClock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

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

    [HttpGet("status")]
    public async Task<ActionResult<ClockStatusDto>> GetClockStatus(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetClockStatusQuery(), cancellationToken);
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
