using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.CreateOrder;

public class CreateOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var assignees = await context.Users
            .Where(u => request.AssigneeIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Customer = request.Customer,
            Status = OrderStatus.Backlog,
            CreatedAt = DateTime.UtcNow,
            Assignees = assignees
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.Title,
            order.Description,
            order.Customer,
            order.Status,
            order.CreatedAt,
            assignees.Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName)).ToList());
    }
}
