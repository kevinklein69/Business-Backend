using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Orders.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<OrderDto>;
