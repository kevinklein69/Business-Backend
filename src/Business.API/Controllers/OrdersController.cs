using Business.Application.Common.Exceptions;
using Business.Application.Features.Orders;
using Business.Application.Features.Orders.CreateOrder;
using Business.Application.Features.Orders.GetOrders;
using Business.Application.Features.Orders.UpdateOrder;
using Business.Application.Features.Orders.UpdateOrderStatus;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController(ISender sender) : ControllerBase
{
    public record UpsertOrderRequest(
        string Title,
        string? Description,
        string? Customer,
        List<Guid> AssigneeIds,
        decimal? Revenue,
        DateOnly? InvoiceDate,
        decimal? EstimatedHours,
        DateOnly? PlannedStartDate,
        DateOnly? PlannedEndDate,
        decimal? ActualHours,
        string? DeviationReason);

    public record UpdateStatusRequest(OrderStatus Status);

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrdersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(UpsertOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateOrderCommand(
                request.Title,
                request.Description,
                request.Customer,
                request.AssigneeIds,
                request.Revenue,
                request.InvoiceDate,
                request.EstimatedHours,
                request.PlannedStartDate,
                request.PlannedEndDate,
                request.ActualHours,
                request.DeviationReason),
            cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrderDto>> Update(Guid id, UpsertOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new UpdateOrderCommand(
                    id,
                    request.Title,
                    request.Description,
                    request.Customer,
                    request.AssigneeIds,
                    request.Revenue,
                    request.InvoiceDate,
                    request.EstimatedHours,
                    request.PlannedStartDate,
                    request.PlannedEndDate,
                    request.ActualHours,
                    request.DeviationReason),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateOrderStatusCommand(id, request.Status), cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
