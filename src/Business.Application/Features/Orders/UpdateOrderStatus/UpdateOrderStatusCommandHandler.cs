using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Assignees)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        order.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.Title,
            order.Description,
            order.Customer,
            order.Status,
            order.CreatedAt,
            order.Revenue,
            order.InvoiceDate,
            order.EstimatedHours,
            order.PlannedStartDate,
            order.PlannedEndDate,
            order.ActualHours,
            order.DeviationReason,
            order.Assignees.Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName)).ToList());
    }
}
