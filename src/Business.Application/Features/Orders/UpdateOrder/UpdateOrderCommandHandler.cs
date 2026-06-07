using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.UpdateOrder;

public class UpdateOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Assignees)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var assignees = await context.Users
            .Where(u => request.AssigneeIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        order.Title = request.Title;
        order.Description = request.Description;
        order.Customer = request.Customer;
        order.Assignees = assignees;

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
