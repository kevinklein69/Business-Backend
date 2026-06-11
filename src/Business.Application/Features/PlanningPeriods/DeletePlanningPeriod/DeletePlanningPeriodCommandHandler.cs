using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.PlanningPeriods.DeletePlanningPeriod;

public class DeletePlanningPeriodCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeletePlanningPeriodCommand>
{
    public async Task Handle(DeletePlanningPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await context.PlanningPeriods
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PlanningPeriod), request.Id);

        // Orders referencing this period are detached via the SetNull FK behaviour, but we clear
        // them explicitly so the change is tracked even for already-loaded entities.
        var orders = await context.Orders
            .Where(o => o.PlanningPeriodId == period.Id)
            .ToListAsync(cancellationToken);

        foreach (var order in orders)
        {
            order.PlanningPeriodId = null;
        }

        context.PlanningPeriods.Remove(period);
        await context.SaveChangesAsync(cancellationToken);
    }
}
