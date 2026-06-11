using MediatR;

namespace Business.Application.Features.PlanningPeriods.GetPlanningPeriods;

public record GetPlanningPeriodsQuery : IRequest<List<PlanningPeriodDto>>;
