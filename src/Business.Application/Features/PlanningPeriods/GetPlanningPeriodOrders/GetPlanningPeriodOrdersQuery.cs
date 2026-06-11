using Business.Application.Features.Orders;
using MediatR;

namespace Business.Application.Features.PlanningPeriods.GetPlanningPeriodOrders;

/// Lazily loads the orders of a single planning period (used when expanding a closed period).
public record GetPlanningPeriodOrdersQuery(Guid PeriodId) : IRequest<List<OrderDto>>;
