using MediatR;

namespace Business.Application.Features.PlanningPeriods.DeletePlanningPeriod;

/// Deletes a planning period. Its orders fall back to "Planung" / unassigned (FK is set to null).
public record DeletePlanningPeriodCommand(Guid Id) : IRequest;
