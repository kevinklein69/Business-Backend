using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.GetEntries;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class GetEntriesQueryHandlerTests
{
    [Fact]
    public async Task GetEntries_IncludesPendingManualEntries()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(today, new TimeOnly(9, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(today, new TimeOnly(17, 0)),
            Status = TimeEntryStatus.Pending,
            IsManual = true,
            Note = "Vergessen einzustempeln"
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var handler = new GetEntriesQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetEntriesQuery(today.Year, today.Month), CancellationToken.None);

        var entry = Assert.Single(result);
        Assert.True(entry.IsManual);
        Assert.Equal(TimeEntryStatus.Pending, entry.Status);
        Assert.Equal("Vergessen einzustempeln", entry.Note);
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
