using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.PlanningPeriods.GetPlanningPeriods;

public class GetPlanningPeriodsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPlanningPeriodsQuery, List<PlanningPeriodDto>>
{
    public async Task<List<PlanningPeriodDto>> Handle(GetPlanningPeriodsQuery request, CancellationToken cancellationToken)
    {
        var periods = await context.PlanningPeriods
            .Select(p => new PlanningPeriodDto(
                p.Id,
                p.Name,
                p.StartDate,
                p.EndDate,
                p.Status,
                p.Orders.Count))
            .ToListAsync(cancellationToken);

        // Active first, then planned, then closed; within each group by start date.
        return periods
            .OrderBy(p => p.Status switch
            {
                PlanningPeriodStatus.Active => 0,
                PlanningPeriodStatus.Planned => 1,
                _ => 2
            })
            .ThenBy(p => p.StartDate ?? DateOnly.MaxValue)
            .ToList();
    }
}
