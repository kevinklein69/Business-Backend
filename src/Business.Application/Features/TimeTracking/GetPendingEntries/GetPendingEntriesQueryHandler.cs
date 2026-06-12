using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.GetPendingEntries;

public class GetPendingEntriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPendingEntriesQuery, List<PendingTimeEntryDto>>
{
    public async Task<List<PendingTimeEntryDto>> Handle(GetPendingEntriesQuery request, CancellationToken cancellationToken)
    {
        var entries = await context.TimeEntries
            .Include(t => t.User)
            .Where(t => t.Status == TimeEntryStatus.Pending)
            .OrderBy(t => t.ClockIn)
            .ToListAsync(cancellationToken);

        return entries
            .Select(t => new PendingTimeEntryDto(
                t.Id,
                t.UserId,
                $"{t.User.FirstName} {t.User.LastName}",
                DateOnly.FromDateTime(t.ClockIn),
                t.ClockIn,
                t.ClockOut,
                t.IsManual,
                t.Status,
                t.Note))
            .ToList();
    }
}
