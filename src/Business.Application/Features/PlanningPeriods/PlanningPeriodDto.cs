using Business.Domain.Enums;

namespace Business.Application.Features.PlanningPeriods;

public record PlanningPeriodDto(
    Guid Id,
    string Name,
    DateOnly? StartDate,
    DateOnly? EndDate,
    PlanningPeriodStatus Status,
    int OrderCount);
