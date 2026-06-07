using Betrieb.Domain.Enums;

namespace Betrieb.Application.Features.Absence;

public record AbsenceRequestDto(
    Guid Id,
    Guid UserId,
    string UserName,
    AbsenceType Type,
    DateOnly StartDate,
    DateOnly EndDate,
    int BusinessDays,
    AbsenceStatus Status,
    string? Comment);

public static class AbsenceRequestExtensions
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

    public static AbsenceRequestDto ToDto(this Domain.Entities.AbsenceRequest request) =>
        new(
            request.Id,
            request.UserId,
            $"{request.User.FirstName} {request.User.LastName}",
            request.Type,
            request.StartDate,
            request.EndDate,
            CountBusinessDays(request.StartDate, request.EndDate),
            request.Status,
            request.Comment);
}
