using MediatR;

namespace Business.Application.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(
    Guid Id,
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
    string? DeviationReason) : IRequest<OrderDto>;
