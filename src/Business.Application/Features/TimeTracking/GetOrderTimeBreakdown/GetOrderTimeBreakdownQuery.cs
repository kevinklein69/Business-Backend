using MediatR;

namespace Business.Application.Features.TimeTracking.GetOrderTimeBreakdown;

public record GetOrderTimeBreakdownQuery(Guid OrderId) : IRequest<List<OrderTimeBreakdownEntryDto>>;

public record OrderTimeBreakdownEntryDto(Guid UserId, string UserName, int NetMinutes);
