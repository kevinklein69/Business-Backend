using MediatR;

namespace Business.Application.Features.PlanningPeriods.CreatePlanningPeriod;

public record CreatePlanningPeriodCommand(string Name, DateOnly? StartDate, DateOnly? EndDate)
    : IRequest<PlanningPeriodDto>;
