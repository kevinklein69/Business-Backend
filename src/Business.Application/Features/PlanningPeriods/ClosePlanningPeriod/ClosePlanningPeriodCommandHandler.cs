using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.PlanningPeriods.ClosePlanningPeriod;

public class ClosePlanningPeriodCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ClosePlanningPeriodCommand, PlanningPeriodDto>
{
    public async Task<PlanningPeriodDto> Handle(ClosePlanningPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await context.PlanningPeriods
            .Include(p => p.Orders)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PlanningPeriod), request.Id);

        Guid? targetPeriodId = null;
        if (request.ReassignTarget == ReassignTarget.NextPeriod)
        {
            targetPeriodId = await ResolveNextPeriodId(period, request.TargetPeriodId, cancellationToken);
        }

        // Unerledigte Aufträge umhängen, erledigte bleiben im (nun abgeschlossenen) Zeitraum.
        foreach (var order in period.Orders.Where(o => o.Status != OrderStatus.Done).ToList())
        {
            order.PlanningPeriodId = targetPeriodId;
        }

        period.Status = PlanningPeriodStatus.Closed;
        await context.SaveChangesAsync(cancellationToken);

        var orderCount = await context.Orders.CountAsync(o => o.PlanningPeriodId == period.Id, cancellationToken);

        return new PlanningPeriodDto(period.Id, period.Name, period.StartDate, period.EndDate, period.Status, orderCount);
    }

    private async Task<Guid?> ResolveNextPeriodId(PlanningPeriod closing, Guid? explicitTargetId, CancellationToken cancellationToken)
    {
        if (explicitTargetId is { } id)
        {
            var exists = await context.PlanningPeriods.AnyAsync(p => p.Id == id && p.Id != closing.Id, cancellationToken);
            return exists ? id : null;
        }

        // Earliest planned period that starts on/after the closing period (fallback: any planned period).
        var candidates = await context.PlanningPeriods
            .Where(p => p.Id != closing.Id && p.Status == PlanningPeriodStatus.Planned)
            .ToListAsync(cancellationToken);

        var next = candidates
            .OrderBy(p => p.StartDate ?? DateOnly.MaxValue)
            .FirstOrDefault(p => closing.StartDate == null || p.StartDate == null || p.StartDate >= closing.StartDate)
            ?? candidates.OrderBy(p => p.StartDate ?? DateOnly.MaxValue).FirstOrDefault();

        return next?.Id;
    }
}
