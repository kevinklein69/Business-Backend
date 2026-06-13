using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.UpdateOrderPlanningPeriod;

public class UpdateOrderPlanningPeriodCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateOrderPlanningPeriodCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderPlanningPeriodCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Assignees)
            .Include(o => o.Attachments)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        if (request.PlanningPeriodId is { } periodId)
        {
            var exists = await context.PlanningPeriods.AnyAsync(p => p.Id == periodId, cancellationToken);
            if (!exists)
            {
                throw new ValidationException(
                [
                    new ValidationFailure("PlanningPeriodId", "Der angegebene Planungszeitraum wurde nicht gefunden.")
                ]);
            }
        }

        order.PlanningPeriodId = request.PlanningPeriodId;

        await context.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
