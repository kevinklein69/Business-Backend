using MediatR;

namespace Business.Application.Features.Orders.DeleteOrder;

public record DeleteOrderCommand(Guid Id) : IRequest;
