using Business.Application.Common.Exceptions;
using Business.Application.Features.Orders;
using Business.Application.Features.PlanningPeriods;
using Business.Application.Features.PlanningPeriods.ClosePlanningPeriod;
using Business.Application.Features.PlanningPeriods.CreatePlanningPeriod;
using Business.Application.Features.PlanningPeriods.DeletePlanningPeriod;
using Business.Application.Features.PlanningPeriods.GetPlanningPeriodOrders;
using Business.Application.Features.PlanningPeriods.GetPlanningPeriods;
using Business.Application.Features.PlanningPeriods.UpdatePlanningPeriod;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/planning-periods")]
public class PlanningPeriodsController(ISender sender) : ControllerBase
{
    public record UpsertPlanningPeriodRequest(string Name, DateOnly? StartDate, DateOnly? EndDate);

    public record UpdatePlanningPeriodRequest(string Name, DateOnly? StartDate, DateOnly? EndDate, PlanningPeriodStatus Status);

    public record ClosePlanningPeriodRequest(ReassignTarget ReassignTarget, Guid? TargetPeriodId);

    [HttpGet]
    public async Task<ActionResult<List<PlanningPeriodDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPlanningPeriodsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/orders")]
    public async Task<ActionResult<List<OrderDto>>> GetOrders(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPlanningPeriodOrdersQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PlanningPeriodDto>> Create(UpsertPlanningPeriodRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePlanningPeriodCommand(request.Name, request.StartDate, request.EndDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PlanningPeriodDto>> Update(Guid id, UpdatePlanningPeriodRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new UpdatePlanningPeriodCommand(id, request.Name, request.StartDate, request.EndDate, request.Status),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/close")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PlanningPeriodDto>> Close(Guid id, ClosePlanningPeriodRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new ClosePlanningPeriodCommand(id, request.ReassignTarget, request.TargetPeriodId),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeletePlanningPeriodCommand(id), cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
