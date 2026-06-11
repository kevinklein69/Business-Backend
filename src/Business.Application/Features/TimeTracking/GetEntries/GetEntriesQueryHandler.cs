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

        return entries
            .Select(t =>
            {
                var gross = (int)(t.ClockOut!.Value - t.ClockIn).TotalMinutes;
                var brk   = CalculateBreakMinutes(gross);
                return new TimeEntryDto(
                    DateOnly.FromDateTime(t.ClockIn),
                    t.ClockIn,
                    t.ClockOut!.Value,
                    gross,
                    brk,
                    gross - brk);
            })
            .ToList();
    }

    // ArbSchG §4: 30 min break from 6 h, 45 min from more than 9 h.
    private static int CalculateBreakMinutes(int grossMinutes) => grossMinutes switch
    {
        > 540 => 45,
        >= 360 => 30,
        _ => 0,
    };
}
