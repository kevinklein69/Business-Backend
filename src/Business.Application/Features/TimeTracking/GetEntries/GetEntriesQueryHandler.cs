using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.GetEntries;

public class GetEntriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetEntriesQuery, List<TimeEntryDto>>
{
    public async Task<List<TimeEntryDto>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var monthStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd   = monthStart.AddMonths(1);

        var entries = await context.TimeEntries
            .Where(t => t.UserId == userId
                     && t.ClockOut != null
                     && t.ClockIn >= monthStart
                     && t.ClockIn < monthEnd)
            .OrderByDescending(t => t.ClockIn)
            .ToListAsync(cancellationToken);

        return entries.Select(t => t.ToDto()).ToList();
    }
}
