using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
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

        if (assignees.Count != request.AssigneeIds.Distinct().Count())
        {
            throw new ValidationException(
            [
                new ValidationFailure("AssigneeIds", "Mindestens ein zugewiesener Mitarbeiter wurde nicht gefunden.")
            ]);
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Customer = request.Customer,
            Status = OrderStatus.ToDo,
            CreatedAt = DateTime.UtcNow,
            Revenue = request.Revenue,
            InvoiceDate = request.InvoiceDate,
            EstimatedHours = request.EstimatedHours,
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            ActualHours = request.ActualHours,
            DeviationReason = request.DeviationReason,
            Assignees = assignees,
            Positions = request.Positions
                .Select((p, index) => new OrderPosition
                {
                    Id = Guid.NewGuid(),
                    Description = p.Description,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    SortOrder = index
                })
                .ToList()
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
            order.PlanningPeriodId,
            order.Revenue,
            order.InvoiceDate,
            order.EstimatedHours,
            order.PlannedStartDate,
            order.PlannedEndDate,
            order.ActualHours,
            order.DeviationReason,
            assignees.Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName)).ToList(),
            order.Positions
                .OrderBy(p => p.SortOrder)
                .Select(p => new OrderPositionDto(p.Id, p.Description, p.Quantity, p.UnitPrice, p.SortOrder))
                .ToList(),
            []);
    }
}
