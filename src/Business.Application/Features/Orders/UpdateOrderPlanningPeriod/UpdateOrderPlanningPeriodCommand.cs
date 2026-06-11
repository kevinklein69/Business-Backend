using MediatR;

namespace Business.Application.Features.Orders.UpdateOrderPlanningPeriod;

/// Assigns an order to a planning period (sprint). A null <paramref name="PlanningPeriodId"/>
/// moves the order back to "Planung" (unassigned).
public record UpdateOrderPlanningPeriodCommand(Guid Id, Guid? PlanningPeriodId) : IRequest<OrderDto>;
