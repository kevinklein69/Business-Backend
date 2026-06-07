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
    List<AssigneeDto> Assignees);
