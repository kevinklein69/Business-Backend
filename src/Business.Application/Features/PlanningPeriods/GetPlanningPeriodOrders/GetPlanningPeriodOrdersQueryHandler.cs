using Business.Application.Common.Interfaces;
using Business.Application.Features.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.PlanningPeriods.GetPlanningPeriodOrders;

public class GetPlanningPeriodOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPlanningPeriodOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetPlanningPeriodOrdersQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders
            .Where(o => o.PlanningPeriodId == request.PeriodId)
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
                o.Attachments
                    .OrderBy(a => a.UploadedAt)
                    .Select(a => new OrderAttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAt))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
