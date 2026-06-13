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

        if (request.PlanningPeriodId is { } periodId)
        {
            var periodExists = await context.PlanningPeriods.AnyAsync(p => p.Id == periodId, cancellationToken);
            if (!periodExists)
            {
                throw new ValidationException(
                [
                    new ValidationFailure("PlanningPeriodId", "Der angegebene Planungszeitraum wurde nicht gefunden.")
                ]);
            }
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Customer = request.Customer,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            Zip = request.Zip,
            City = request.City,
            Status = OrderStatus.ToDo,
            CreatedAt = DateTime.UtcNow,
            PlanningPeriodId = request.PlanningPeriodId,
            Revenue = request.Revenue,
            InvoiceDate = request.InvoiceDate,
            EstimatedHours = request.EstimatedHours,
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            DeviationReason = request.DeviationReason,
            Assignees = assignees
        };

        context.Orders.Add(order);
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
            []);
    }
}
