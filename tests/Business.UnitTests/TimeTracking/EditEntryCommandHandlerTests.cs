using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.EditEntry;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class EditEntryCommandHandlerTests
{
    private static readonly DateOnly Yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

    [Fact]
    public async Task Edit_TimeChange_ResetsToPendingAndMarksManual()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var entryId = Guid.NewGuid();
        context.TimeEntries.Add(new TimeEntry
        {
            Id = entryId,
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(16, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var handler = new EditEntryCommandHandler(context, currentUser);

        // End time corrected from 16:00 to 17:00.
        var command = new EditEntryCommand(entryId, Yesterday, new TimeOnly(8, 0), new TimeOnly(17, 0), "Korrektur Feierabend");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(TimeEntryStatus.Pending, result.Status);
        Assert.True(result.IsManual);
        Assert.Equal("Korrektur Feierabend", result.Note);
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
