using MediatR;

namespace Business.Application.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(
    Guid Id,
    string Title,
    string? Description,
    string? Customer,
    string Street,
    string HouseNumber,
    string Zip,
    string City,
    List<Guid> AssigneeIds,
    decimal? Revenue,
    DateOnly? InvoiceDate,
    decimal? EstimatedHours,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    decimal? ActualHours,
    string? DeviationReason,
    List<OrderPositionInput> Positions) : IRequest<OrderDto>;
