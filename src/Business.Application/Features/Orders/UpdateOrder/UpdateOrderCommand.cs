using MediatR;

namespace Business.Application.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(
    Guid Id,
    string Title,
    string? Description,
    string? Customer,
    List<Guid> AssigneeIds) : IRequest<OrderDto>;
