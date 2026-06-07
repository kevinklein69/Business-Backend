using Betrieb.Domain.Enums;

namespace Betrieb.Application.Features.Orders;

public record AssigneeDto(Guid Id, string Name);

public record OrderDto(
    Guid Id,
    string Title,
    string? Description,
    string? Customer,
    OrderStatus Status,
    DateTime CreatedAt,
    List<AssigneeDto> Assignees);
