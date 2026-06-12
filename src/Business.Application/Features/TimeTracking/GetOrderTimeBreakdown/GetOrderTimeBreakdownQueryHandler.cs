using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.GetOrderTimeBreakdown;

/// Per-employee breakdown of net minutes worked on an order, used to show "who worked how long" on the order.
public class GetOrderTimeBreakdownQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrderTimeBreakdownQuery, List<OrderTimeBreakdownEntryDto>>
{
    public async Task<List<OrderTimeBreakdownEntryDto>> Handle(GetOrderTimeBreakdownQuery request, CancellationToken cancellationToken)
    {
        var entries = await context.TimeEntries
            .Where(t => t.OrderId == request.OrderId && t.ClockOut != null && t.Status == TimeEntryStatus.Approved)
            .Include(t => t.User)
            .ToListAsync(cancellationToken);

        return entries
            .GroupBy(t => t.User)
            .Select(g => new OrderTimeBreakdownEntryDto(
                g.Key.Id,
                $"{g.Key.FirstName} {g.Key.LastName}",
                g.Sum(t =>
                {
                    var gross = (int)(t.ClockOut!.Value - t.ClockIn).TotalMinutes;
                    return gross - TimeEntryExtensions.CalculateBreakMinutes(gross);
                })))
            .OrderByDescending(e => e.NetMinutes)
            .ToList();
    }
}
