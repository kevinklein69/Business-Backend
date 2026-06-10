using MediatR;

namespace Business.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
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
