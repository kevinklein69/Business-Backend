using Business.Domain.Common;
using Business.Domain.Enums;

namespace Business.Application.Features.Absence;

public record AbsenceRequestDto(
    Guid Id,
    Guid UserId,
    string UserName,
    AbsenceType Type,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal BusinessDays,
    AbsenceStatus Status,
    string? Comment);

public static class AbsenceRequestExtensions
{
    /// Inclusive count of working days between start and end: weekends and full public
    /// holidays (for the company's Bundesland) don't count, and Heiligabend/Silvester
    /// count as half days, matching common German employer practice.
    public static decimal CountBusinessDays(DateOnly start, DateOnly end, GermanState state)
    {
        if (end < start) return 0;

        var days = 0m;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            if (GermanHolidays.IsFullHoliday(date, state)) continue;

            days += GermanHolidays.IsHalfDay(date) ? 0.5m : 1m;
        }

        return days;
    }

    public static AbsenceRequestDto ToDto(this Domain.Entities.AbsenceRequest request, GermanState state) =>
        new(
            request.Id,
            request.UserId,
            $"{request.User.FirstName} {request.User.LastName}",
            request.Type,
            request.StartDate,
            request.EndDate,
            CountBusinessDays(request.StartDate, request.EndDate, state),
            request.Status,
            request.Comment);
}
