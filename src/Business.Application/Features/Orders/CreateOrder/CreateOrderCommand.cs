using MediatR;

namespace Business.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
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
    string? DeviationReason,
    List<OrderPositionInput> Positions,
    Guid? PlanningPeriodId = null) : IRequest<OrderDto>;
