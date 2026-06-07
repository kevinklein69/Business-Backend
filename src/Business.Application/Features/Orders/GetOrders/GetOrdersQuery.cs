using MediatR;

namespace Business.Application.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>;
