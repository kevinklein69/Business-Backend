using Betrieb.Domain.Enums;

namespace Betrieb.Application.Features.Vacation;

public record VacationRequestDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    int BusinessDays,
    VacationStatus Status,
    string? Comment);

public static class VacationRequestExtensions
{
    /// Inclusive count of Mon-Fri days between start and end, mirroring the frontend's
    /// `differenceInBusinessDays(bis, von) + 1` calculation.
    public static int CountBusinessDays(DateOnly start, DateOnly end)
    {
        if (end < start) return 0;

        var days = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
            {
                days++;
            }
        }

        return days;
    }
}
