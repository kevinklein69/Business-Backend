using Business.Application.Common.Interfaces;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetEmployeeEntries;

public class GetEmployeeEntriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmployeeEntriesQuery, List<TimeEntryDto>>
{
    public async Task<List<TimeEntryDto>> Handle(GetEmployeeEntriesQuery request, CancellationToken cancellationToken)
    {
        return await TimeEntryQueries.GetEntriesAsync(context, request.UserId, request.Year, request.Month, cancellationToken);
    }
}
