using MediatR;

namespace Business.Application.Features.PlanningPeriods.ClosePlanningPeriod;

/// Where unfinished (non-Done) orders go when a planning period is closed.
public enum ReassignTarget
{
    Unassigned,
    NextPeriod
}

/// Closes a planning period and moves its unfinished orders either to the unassigned pool or to the
/// next planned period. When <see cref="ReassignTarget.NextPeriod"/> is chosen, an explicit
/// <paramref name="TargetPeriodId"/> may be given; otherwise the next planned period by start date
/// is used (falling back to the unassigned pool if none exists).
public record ClosePlanningPeriodCommand(Guid Id, ReassignTarget ReassignTarget, Guid? TargetPeriodId)
    : IRequest<PlanningPeriodDto>;
