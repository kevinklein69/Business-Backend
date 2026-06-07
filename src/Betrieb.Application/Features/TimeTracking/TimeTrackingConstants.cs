namespace Betrieb.Application.Features.TimeTracking;

public static class TimeTrackingConstants
{
    /// Target working minutes per weekday (8h), mirroring the frontend's SOLLZEIT constant.
    public const int DailyTargetMinutes = 480;

    public static bool IsWorkday(DateOnly date) =>
        date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;
}
