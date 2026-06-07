using Betrieb.Application.Common.Exceptions;
using Betrieb.Application.Features.Vacation;
using Betrieb.Application.Features.Vacation.CreateRequest;
using Betrieb.Application.Features.Vacation.GetMyRequests;
using Betrieb.Application.Features.Vacation.UpdateRequestStatus;
using Betrieb.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Betrieb.API.Controllers;

[ApiController]
[Authorize]
[Route("api/vacation-requests")]
public class VacationController(ISender sender) : ControllerBase
{
    public record CreateRequestBody(DateOnly StartDate, DateOnly EndDate, string? Comment);

    public record UpdateStatusRequest(VacationStatus Status);

    [HttpGet]
    public async Task<ActionResult<List<VacationRequestDto>>> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyRequestsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<VacationRequestDto>> Create(CreateRequestBody request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateRequestCommand(request.StartDate, request.EndDate, request.Comment),
            cancellationToken);

        return CreatedAtAction(nameof(GetMine), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<VacationRequestDto>> UpdateStatus(Guid id, UpdateStatusRequest request, CancellationToken cancellationToken)
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
}
