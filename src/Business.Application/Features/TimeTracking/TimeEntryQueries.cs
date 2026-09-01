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
            .Where(t => t.UserId == userId && t.OrderId == null && t.ClockOut != null && t.Status == TimeEntryStatus.Approved)
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

                if (absence.Type == AbsenceType.FlexTimeCompensation)
                {
                    // FlexTime only applies when no real work was done that day — it consumes
                    // 480 min of overtime by recording 0 worked minutes against the daily target.
                    if (!minutesByDay.ContainsKey(date))
                        minutesByDay[date] = 0;
                }
                else
                {
                    // Vacation/sick/etc.: always credit the full daily target on top of any
                    // actual work. If someone works on a vacation day they keep the vacation
                    // credit AND earn overtime for the hours they put in.
                    minutesByDay.TryGetValue(date, out var existing);
                    minutesByDay[date] = existing + TimeTrackingConstants.DailyTargetMinutes;
                }
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

        // Determine the start date for the total balance: the earliest day with an actual
        // entry. EntryDate is intentionally NOT used here — it may predate the software's
        // rollout at this customer, which would back-calculate a deficit for days the
        // employee was never expected to clock in with this system.
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        DateOnly? balanceStartDate = minutesByDay.Count > 0 ? minutesByDay.Keys.Min() : null;

        // Enumerate every workday from start up to yesterday (today is still in progress).
        // Days with no clock-ins and no approved absences count as -480 min (full missed day).
        var yesterday = today.AddDays(-1);
        var totalBalanceMinutes = user?.InitialBalanceMinutes ?? 0;
        if (balanceStartDate.HasValue)
        {
            for (var date = balanceStartDate.Value; date <= yesterday; date = date.AddDays(1))
            {
                if (!TimeTrackingConstants.IsWorkday(date)) continue;
                minutesByDay.TryGetValue(date, out var workedMinutes);
                totalBalanceMinutes += workedMinutes - TimeTrackingConstants.DailyTargetMinutes;
            }
        }

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
