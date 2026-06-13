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
            .Include(o => o.Attachments)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var assignees = await context.Users
            .Where(u => request.AssigneeIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        order.Title = request.Title;
        order.Description = request.Description;
        order.Customer = request.Customer;
        order.Street = request.Street;
        order.HouseNumber = request.HouseNumber;
        order.Zip = request.Zip;
        order.City = request.City;
        order.Revenue = request.Revenue;
        order.InvoiceDate = request.InvoiceDate;
        order.EstimatedHours = request.EstimatedHours;
        order.PlannedStartDate = request.PlannedStartDate;
        order.PlannedEndDate = request.PlannedEndDate;
        order.DeviationReason = request.DeviationReason;
        order.Assignees = assignees;

        await context.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.Title,
            order.Description,
            order.Customer,
            order.Street,
            order.HouseNumber,
            order.Zip,
            order.City,
            order.Status,
            order.CreatedAt,
            order.PlanningPeriodId,
            order.Revenue,
            order.InvoiceDate,
            order.EstimatedHours,
            order.PlannedStartDate,
            order.PlannedEndDate,
            order.ActualHours,
            order.DeviationReason,
            assignees.Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName)).ToList(),
            order.Attachments
                .OrderBy(a => a.UploadedAt)
                .Select(a => new OrderAttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAt))
                .ToList());
    }
}
