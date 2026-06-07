using MediatR;

namespace Business.Application.Features.TimeTracking.GetClockStatus;

public record GetClockStatusQuery : IRequest<ClockStatusDto>;

public record ClockStatusDto(bool IsClockedIn, DateTime? ClockIn);
