using Business.Domain.Enums;

namespace Business.Application.Features.Orders;

public record AssigneeDto(Guid Id, string Name);

public record OrderDto(
    Guid Id,
    string Title,
    string? Description,
    string? Customer,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal? Revenue,
    DateOnly? InvoiceDate,
    decimal? EstimatedHours,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    decimal? ActualHours,
    string? DeviationReason,
    List<AssigneeDto> Assignees);
