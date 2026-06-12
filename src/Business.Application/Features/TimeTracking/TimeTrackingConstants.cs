namespace Business.Application.Features.TimeTracking;

public static class TimeTrackingConstants
{
    /// Target working minutes per weekday (8h), mirroring the frontend's SOLLZEIT constant.
    public const int DailyTargetMinutes = 480;

    /// Employees may only create/edit manual time entries for dates within this many days
    /// in the past. Admins and Managers are exempt from this limit.
    public const int MaxBackdateDays = 30;

    /// Maximum length of the mandatory reason/note for manual entries and corrections.
    public const int MaxNoteLength = 500;

    public static bool IsWorkday(DateOnly date) =>
        date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;

    /// DateOnly.ToDateTime(TimeOnly) returns DateTimeKind.Unspecified; TimeEntry times are UTC.
    public static DateTime ToUtc(DateOnly date, TimeOnly time) =>
        DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Utc);

    /// The edit form only captures HH:mm, while a real clock-in/out has seconds/ms precision.
    /// Truncate to minute precision so re-submitting the same displayed time isn't seen as a change.
    public static DateTime TruncateToMinute(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
}
