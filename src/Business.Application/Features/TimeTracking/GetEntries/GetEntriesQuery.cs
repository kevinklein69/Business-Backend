using MediatR;

namespace Business.Application.Features.TimeTracking.GetEntries;

public record GetEntriesQuery(int Year, int Month) : IRequest<List<TimeEntryDto>>;

public record TimeEntryDto(
    DateOnly Date,
    DateTime ClockIn,
    DateTime ClockOut,
    int GrossDurationMinutes,
    int BreakMinutes,
    int NetDurationMinutes);
