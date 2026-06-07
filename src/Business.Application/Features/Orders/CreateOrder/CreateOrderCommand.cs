using MediatR;

namespace Business.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    string Title,
    string? Description,
    string? Customer,
    List<Guid> AssigneeIds) : IRequest<OrderDto>;
