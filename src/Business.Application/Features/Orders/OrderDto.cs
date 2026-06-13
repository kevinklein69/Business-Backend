using Business.Domain.Enums;

namespace Business.Application.Features.Orders;

public record AssigneeDto(Guid Id, string Name);

public record OrderAttachmentDto(Guid Id, string FileName, string ContentType, long SizeBytes, DateTime UploadedAt);

public record OrderDto(
    Guid Id,
    string Title,
    string? Description,
    string? Customer,
    string Street,
    string HouseNumber,
    string Zip,
    string City,
    OrderStatus Status,
    DateTime CreatedAt,
    Guid? PlanningPeriodId,
    decimal? Revenue,
    DateOnly? InvoiceDate,
    decimal? EstimatedHours,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    decimal? ActualHours,
    string? DeviationReason,
    List<AssigneeDto> Assignees,
    List<OrderAttachmentDto> Attachments);
