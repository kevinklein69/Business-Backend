using Betrieb.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.TimeTracking.GetEntries;

public class GetEntriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetEntriesQuery, List<TimeEntryDto>>
{
    private const int MaxResults = 30;

    public async Task<List<TimeEntryDto>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var entries = await context.TimeEntries
            .Where(t => t.UserId == userId && t.ClockOut != null)
            .OrderByDescending(t => t.ClockIn)
            .Take(MaxResults)
            .ToListAsync(cancellationToken);

        return entries
            .Select(t => new TimeEntryDto(
                DateOnly.FromDateTime(t.ClockIn),
                t.ClockIn,
                t.ClockOut!.Value,
                (int)(t.ClockOut!.Value - t.ClockIn).TotalMinutes))
            .ToList();
    }
}
