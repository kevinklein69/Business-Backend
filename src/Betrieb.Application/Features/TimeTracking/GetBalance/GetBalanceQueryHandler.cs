using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.TimeTracking.GetBalance;

public class GetBalanceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetBalanceQuery, BalanceDto>
{
    public async Task<BalanceDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var entries = await context.TimeEntries
            .Where(t => t.UserId == userId && t.ClockOut != null)
            .ToListAsync(cancellationToken);

        // Aggregate worked minutes per calendar day.
        var minutesByDay = entries
            .GroupBy(t => DateOnly.FromDateTime(t.ClockIn))
            .ToDictionary(g => g.Key, g => g.Sum(t => (int)(t.ClockOut!.Value - t.ClockIn).TotalMinutes));

        // Approved absences (Urlaub, Krankheit, Kind krank, ...) count as a full workday (8h)
        // each, on top of/instead of actual clock-ins, so they show up in the statistics too.
        var approvedAbsenceDays = await context.AbsenceRequests
            .Where(a => a.UserId == userId && a.Status == AbsenceStatus.Approved)
            .ToListAsync(cancellationToken);

        foreach (var absence in approvedAbsenceDays)
        {
            for (var date = absence.StartDate; date <= absence.EndDate; date = date.AddDays(1))
            {
                if (!TimeTrackingConstants.IsWorkday(date)) continue;
                if (minutesByDay.ContainsKey(date)) continue;
                minutesByDay[date] = TimeTrackingConstants.DailyTargetMinutes;
            }
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday) weekStart = today.AddDays(-6);
        var weekEnd = weekStart.AddDays(6);

        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var weekMinutes = minutesByDay
            .Where(kv => kv.Key >= weekStart && kv.Key <= weekEnd)
            .Sum(kv => kv.Value);

        var monthMinutes = minutesByDay
            .Where(kv => kv.Key >= monthStart && kv.Key <= monthEnd)
            .Sum(kv => kv.Value);

        var weekTargetMinutes = 5 * TimeTrackingConstants.DailyTargetMinutes;

        var monthWorkdays = Enumerable.Range(0, monthEnd.DayNumber - monthStart.DayNumber + 1)
            .Select(monthStart.AddDays)
            .Count(TimeTrackingConstants.IsWorkday);
        var monthTargetMinutes = monthWorkdays * TimeTrackingConstants.DailyTargetMinutes;

        var totalBalanceMinutes = minutesByDay.Sum(kv =>
            kv.Value - (TimeTrackingConstants.IsWorkday(kv.Key) ? TimeTrackingConstants.DailyTargetMinutes : 0));

        return new BalanceDto(
            weekMinutes,
            weekTargetMinutes,
            monthMinutes,
            monthTargetMinutes,
            totalBalanceMinutes);
    }
}
