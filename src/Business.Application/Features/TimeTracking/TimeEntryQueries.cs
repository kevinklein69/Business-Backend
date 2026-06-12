using Business.Application.Common.Interfaces;
using Business.Application.Features.TimeTracking.GetBalance;
using Business.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking;

/// Shared time-entry/balance queries, used both for the current user's own data
/// and for the Admin/Manager "view employee" endpoints.
public static class TimeEntryQueries
{
    public static async Task<List<TimeEntryDto>> GetEntriesAsync(
        IApplicationDbContext context, Guid userId, int year, int month, CancellationToken cancellationToken)
    {
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var entries = await context.TimeEntries
            .Where(t => t.UserId == userId
                     && t.ClockOut != null
                     && t.ClockIn >= monthStart
                     && t.ClockIn < monthEnd)
            .Include(t => t.Order)
            .OrderByDescending(t => t.ClockIn)
            .ToListAsync(cancellationToken);

        return entries.Select(t => t.ToDto()).ToList();
    }

    public static async Task<BalanceDto> GetBalanceAsync(
        IApplicationDbContext context, Guid userId, CancellationToken cancellationToken)
    {
        var entries = await context.TimeEntries
            .Where(t => t.UserId == userId && t.ClockOut != null && t.Status == TimeEntryStatus.Approved)
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

                // FlexTimeCompensation days consume overtime: record 0 worked minutes so the
                // daily target is subtracted from the balance. All other absence types (Vacation,
                // Sick, ChildSick) are neutral — they fill the target to keep the balance at zero.
                minutesByDay[date] = absence.Type == AbsenceType.FlexTimeCompensation
                    ? 0
                    : TimeTrackingConstants.DailyTargetMinutes;
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

    /// Recalculates Order.ActualHours from the net minutes of all completed, approved
    /// order-related time entries (across all employees). Does not call SaveChanges.
    public static async Task RecalculateOrderActualHoursAsync(
        IApplicationDbContext context, Guid orderId, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null) return;

        var entries = await context.TimeEntries
            .Where(t => t.OrderId == orderId && t.ClockOut != null && t.Status == TimeEntryStatus.Approved)
            .ToListAsync(cancellationToken);

        var totalNetMinutes = entries.Sum(t =>
        {
            var gross = (int)(t.ClockOut!.Value - t.ClockIn).TotalMinutes;
            return gross - TimeEntryExtensions.CalculateBreakMinutes(gross);
        });

        order.ActualHours = Math.Round(totalNetMinutes / 60m, 2);
    }
}
