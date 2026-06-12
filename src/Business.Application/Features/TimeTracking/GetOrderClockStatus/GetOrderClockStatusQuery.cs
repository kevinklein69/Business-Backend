using MediatR;

namespace Business.Application.Features.TimeTracking.GetOrderClockStatus;

public record GetOrderClockStatusQuery(Guid OrderId) : IRequest<OrderClockStatusDto>;

public record OrderClockStatusDto(bool IsClockedIn, DateTime? ClockIn);
