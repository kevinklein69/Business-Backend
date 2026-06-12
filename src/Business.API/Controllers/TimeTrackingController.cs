using Business.Application.Common.Exceptions;
using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.CreateManualEntry;
using Business.Application.Features.TimeTracking.EditEntry;
using Business.Application.Features.TimeTracking.GetBalance;
using Business.Application.Features.TimeTracking.GetClockStatus;
using Business.Application.Features.TimeTracking.GetEmployeeBalance;
using Business.Application.Features.TimeTracking.GetEmployeeEntries;
using Business.Application.Features.TimeTracking.GetEntries;
using Business.Application.Features.TimeTracking.GetPendingEntries;
using Business.Application.Features.TimeTracking.ToggleClock;
using Business.Application.Features.TimeTracking.UpdateEntryStatus;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/time-tracking")]
public class TimeTrackingController(ISender sender) : ControllerBase
{
    public record ManualEntryBody(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string Note);

    public record EditEntryBody(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string Note);

    public record UpdateEntryStatusBody(TimeEntryStatus Status);

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
    public async Task<ActionResult<List<TimeEntryDto>>> GetEntries(
        [FromQuery] int? year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var result = await sender.Send(new GetEntriesQuery(year ?? now.Year, month ?? now.Month), cancellationToken);
        return Ok(result);
    }

    [HttpGet("balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBalanceQuery(), cancellationToken);
        return Ok(result);
    }

    /// Manager/Admin: time entries of a specific employee for a given month.
    [HttpGet("entries/{userId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<TimeEntryDto>>> GetEmployeeEntries(
        Guid userId,
        [FromQuery] int? year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var result = await sender.Send(new GetEmployeeEntriesQuery(userId, year ?? now.Year, month ?? now.Month), cancellationToken);
        return Ok(result);
    }

    /// Manager/Admin: time account balance of a specific employee.
    [HttpGet("balance/{userId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BalanceDto>> GetEmployeeBalance(Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmployeeBalanceQuery(userId), cancellationToken);
        return Ok(result);
    }

    /// Employee creates a manual time entry (e.g. forgot to clock in/out). Starts as Pending.
    [HttpPost("manual")]
    public async Task<ActionResult<TimeEntryDto>> CreateManual(ManualEntryBody request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateManualEntryCommand(request.Date, request.StartTime, request.EndTime, request.Note),
            cancellationToken);

        return Ok(result);
    }

    /// Employee corrects an existing entry (own entries only). Resets status to Pending for re-review.
    [HttpPut("edit/{id:guid}")]
    public async Task<ActionResult<TimeEntryDto>> Edit(Guid id, EditEntryBody request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new EditEntryCommand(id, request.Date, request.StartTime, request.EndTime, request.Note),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// Manager/Admin overview of time entries awaiting approval across all employees.
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<PendingTimeEntryDto>>> GetPending(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPendingEntriesQuery(), cancellationToken);
        return Ok(result);
    }

    /// Manager/Admin approves or rejects a pending time entry.
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<TimeEntryDto>> UpdateStatus(Guid id, UpdateEntryStatusBody request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateEntryStatusCommand(id, request.Status), cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
