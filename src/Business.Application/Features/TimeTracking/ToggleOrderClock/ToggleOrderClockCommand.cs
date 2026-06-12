using MediatR;

namespace Business.Application.Features.TimeTracking.ToggleOrderClock;

public record ToggleOrderClockCommand(Guid OrderId) : IRequest<ToggleOrderClockResult>;

public record ToggleOrderClockResult(bool IsClockedIn, DateTime? ClockIn);
