using Betrieb.Domain.Enums;
using MediatR;

namespace Betrieb.Application.Features.Orders.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<OrderDto>;
