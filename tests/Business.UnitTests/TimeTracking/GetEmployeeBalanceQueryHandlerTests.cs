using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.GetEmployeeBalance;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class GetEmployeeBalanceQueryHandlerTests
{
    private static readonly DateOnly WorkdayInThePast = PreviousMonday(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-14));

    private static DateOnly PreviousMonday(DateOnly date)
    {
        while (date.DayOfWeek != DayOfWeek.Monday) date = date.AddDays(-1);
        return date;
    }

    [Fact]
    public async Task GetEmployeeBalance_ReturnsBalanceForGivenUser()
    {
        var context = TestDbContextFactory.Create();
        var employeeId = Guid.NewGuid();

        context.Users.Add(new User
        {
            Id = employeeId,
            FirstName = "Erika",
            LastName = "Musterfrau",
            Email = $"{employeeId}@example.com",
            PasswordHash = "hash",
            Role = Role.Employee
        });

        // 10h approved entry against an 8h daily target => +2h overtime for that employee.
        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = employeeId,
            ClockIn = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(18, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var handler = new GetEmployeeBalanceQueryHandler(context);

        var result = await handler.Handle(new GetEmployeeBalanceQuery(employeeId), CancellationToken.None);

        Assert.Equal(120, result.TotalBalanceMinutes);
    }
}
