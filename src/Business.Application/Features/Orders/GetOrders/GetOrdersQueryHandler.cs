using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.GetOrders;

public class GetOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        // Aufträge aus abgeschlossenen Zeiträumen werden lazy via GetPlanningPeriodOrders geladen.
        return await context.Orders
            .Where(o => o.PlanningPeriod == null || o.PlanningPeriod.Status != PlanningPeriodStatus.Closed)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id,
                o.Title,
                o.Description,
                o.Customer,
                o.Street,
                o.HouseNumber,
                o.Zip,
                o.City,
                o.Status,
                o.CreatedAt,
                o.PlanningPeriodId,
                o.Revenue,
                o.InvoiceDate,
                o.EstimatedHours,
                o.PlannedStartDate,
                o.PlannedEndDate,
                o.ActualHours,
                o.DeviationReason,
                o.Assignees
                    .Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName))
                    .ToList(),
                o.Positions
                    .OrderBy(p => p.SortOrder)
                    .Select(p => new OrderPositionDto(p.Id, p.Description, p.Quantity, p.UnitPrice, p.SortOrder))
                    .ToList(),
                o.Attachments
                    .OrderBy(a => a.UploadedAt)
                    .Select(a => new OrderAttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAt))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
