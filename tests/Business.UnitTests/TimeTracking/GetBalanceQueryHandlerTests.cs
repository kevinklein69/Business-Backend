using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.GetBalance;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class GetBalanceQueryHandlerTests
{
    // A Monday in the past, guaranteed to be a workday (Mon-Fri) regardless of when tests run.
    private static readonly DateOnly WorkdayInThePast = PreviousMonday(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-14));

    private static DateOnly PreviousMonday(DateOnly date)
    {
        while (date.DayOfWeek != DayOfWeek.Monday) date = date.AddDays(-1);
        return date;
    }

    [Fact]
    public async Task GetBalance_OnlyCountsApprovedEntries()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(16, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });

        // A second, Pending entry on the same day must not count toward the balance.
        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(17, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(18, 0)),
            Status = TimeEntryStatus.Pending,
            IsManual = true,
            Note = "Vergessen"
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var handler = new GetBalanceQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetBalanceQuery(), CancellationToken.None);

        // 8h approved entry against an 8h daily target => balance of 0 for that day.
        Assert.Equal(0, result.TotalBalanceMinutes);
    }

    [Fact]
    public async Task GetBalance_ExcludesRejectedEntries()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(16, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(17, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(WorkdayInThePast, new TimeOnly(19, 0)),
            Status = TimeEntryStatus.Rejected,
            IsManual = true,
            Note = "Abgelehnt"
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var handler = new GetBalanceQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetBalanceQuery(), CancellationToken.None);

        Assert.Equal(0, result.TotalBalanceMinutes);
    }

    private static async Task SeedUserAsync(BusinessDbContext context, Guid userId)
    {
        context.Users.Add(new User
        {
            Id = userId,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = $"{userId}@example.com",
            PasswordHash = "hash",
            Role = Role.Employee
        });
        await context.SaveChangesAsync();
    }
}
