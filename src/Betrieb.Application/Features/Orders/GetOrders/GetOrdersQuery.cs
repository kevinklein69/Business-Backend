using MediatR;

namespace Betrieb.Application.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>;
