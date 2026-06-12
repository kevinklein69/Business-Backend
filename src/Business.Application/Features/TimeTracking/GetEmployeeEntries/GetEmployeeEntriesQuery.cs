using MediatR;

namespace Business.Application.Features.TimeTracking.GetEmployeeEntries;

public record GetEmployeeEntriesQuery(Guid UserId, int Year, int Month) : IRequest<List<TimeEntryDto>>;
