using Business.Domain.Entities;
using Business.Domain.Enums;

namespace Business.Application.Features.TimeTracking;

public record TimeEntryDto(
    Guid Id,
    DateOnly Date,
    DateTime ClockIn,
    DateTime? ClockOut,
    int GrossDurationMinutes,
    int BreakMinutes,
    int NetDurationMinutes,
    bool IsManual,
    TimeEntryStatus Status,
    string? Note);

public static class TimeEntryExtensions
{
    public static TimeEntryDto ToDto(this TimeEntry entry)
    {
        var gross = entry.ClockOut is null ? 0 : (int)(entry.ClockOut.Value - entry.ClockIn).TotalMinutes;
        var brk = CalculateBreakMinutes(gross);

        return new TimeEntryDto(
            entry.Id,
            DateOnly.FromDateTime(entry.ClockIn),
            entry.ClockIn,
            entry.ClockOut,
            gross,
            brk,
            gross - brk,
            entry.IsManual,
            entry.Status,
            entry.Note);
    }

    // ArbSchG §4: 30 min break from 6 h, 45 min from more than 9 h.
    public static int CalculateBreakMinutes(int grossMinutes) => grossMinutes switch
    {
        > 540 => 45,
        >= 360 => 30,
        _ => 0,
    };
}
