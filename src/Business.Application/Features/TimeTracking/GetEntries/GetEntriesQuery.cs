using MediatR;

namespace Business.Application.Features.TimeTracking.GetEntries;

public record GetEntriesQuery(int Year, int Month) : IRequest<List<TimeEntryDto>>;
