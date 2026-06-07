using MediatR;

namespace Betrieb.Application.Features.TimeTracking.ToggleClock;

public record ToggleClockCommand : IRequest<ToggleClockResult>;

public record ToggleClockResult(bool IsClockedIn, DateTime? ClockIn);
