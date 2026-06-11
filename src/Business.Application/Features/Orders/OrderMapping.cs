using Business.Domain.Entities;

namespace Business.Application.Features.Orders;

/// Shared mapping from a fully-loaded <see cref="Order"/> entity (with Assignees, Positions and
/// Attachments included) to an <see cref="OrderDto"/>. Used by handlers that load and mutate an
/// order entity in memory.
public static class OrderMapping
{
    public static OrderDto ToDto(Order order) => new(
        order.Id,
        order.Title,
        order.Description,
        order.Customer,
        order.Status,
        order.CreatedAt,
        order.PlanningPeriodId,
        order.Revenue,
        order.InvoiceDate,
        order.EstimatedHours,
        order.PlannedStartDate,
        order.PlannedEndDate,
        order.ActualHours,
        order.DeviationReason,
        order.Assignees.Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName)).ToList(),
        order.Positions
            .OrderBy(p => p.SortOrder)
            .Select(p => new OrderPositionDto(p.Id, p.Description, p.Quantity, p.UnitPrice, p.SortOrder))
            .ToList(),
        order.Attachments
            .OrderBy(a => a.UploadedAt)
            .Select(a => new OrderAttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAt))
            .ToList());
}
