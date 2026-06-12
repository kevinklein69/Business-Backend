using Business.Application.Common.Interfaces;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetEntries;

public class GetEntriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetEntriesQuery, List<TimeEntryDto>>
{
    public async Task<List<TimeEntryDto>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        return await TimeEntryQueries.GetEntriesAsync(context, userId, request.Year, request.Month, cancellationToken);
    }
}
