using MediatR;

namespace Betrieb.Application.Features.TimeTracking.GetEntries;

public record GetEntriesQuery : IRequest<List<TimeEntryDto>>;

public record TimeEntryDto(
    DateOnly Date,
    DateTime ClockIn,
    DateTime ClockOut,
    int DurationMinutes);
