using Business.Application.Common.Exceptions;
using Business.Application.Features.Absence;
using Business.Application.Features.Absence.CreateRequest;
using Business.Application.Features.Absence.GetMyRequests;
using Business.Application.Features.Absence.GetTeamRequests;
using Business.Application.Features.Absence.RecordAbsence;
using Business.Application.Features.Absence.UpdateRequestStatus;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/absence-requests")]
public class AbsenceController(ISender sender) : ControllerBase
{
    public record CreateRequestBody(DateOnly StartDate, DateOnly EndDate, string? Comment);

    public record UpdateStatusRequest(AbsenceStatus Status);

    public record RecordAbsenceBody(Guid UserId, AbsenceType Type, DateOnly StartDate, DateOnly EndDate, string? Comment);

    [HttpGet]
    public async Task<ActionResult<List<AbsenceRequestDto>>> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyRequestsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AbsenceRequestDto>> Create(CreateRequestBody request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateRequestCommand(request.StartDate, request.EndDate, request.Comment),
            cancellationToken);

        return CreatedAtAction(nameof(GetMine), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<AbsenceRequestDto>> UpdateStatus(Guid id, UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateRequestStatusCommand(id, request.Status), cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// Manager/Admin overview of absence requests across all employees.
    [HttpGet("team")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<AbsenceRequestDto>>> GetTeam(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTeamRequestsQuery(), cancellationToken);
        return Ok(result);
    }

    /// Manager/Admin records an absence (e.g. sick leave, child sick) on behalf of an employee.
    /// The record is created already approved, since the manager documents a fact.
    [HttpPost("record")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<AbsenceRequestDto>> Record(RecordAbsenceBody request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new RecordAbsenceCommand(request.UserId, request.Type, request.StartDate, request.EndDate, request.Comment),
                cancellationToken);

            return CreatedAtAction(nameof(GetTeam), new { id = result.Id }, result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
