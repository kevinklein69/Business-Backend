using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.PlanningPeriods.UpdatePlanningPeriod;

public record UpdatePlanningPeriodCommand(
    Guid Id,
    string Name,
    DateOnly? StartDate,
    DateOnly? EndDate,
    PlanningPeriodStatus Status) : IRequest<PlanningPeriodDto>;
