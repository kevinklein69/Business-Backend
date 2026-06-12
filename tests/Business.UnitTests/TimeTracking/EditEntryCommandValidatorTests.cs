using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.EditEntry;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class EditEntryCommandValidatorTests
{
    private static readonly DateOnly Yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

    [Fact]
    public async Task Validate_OverlapExcludesEntryBeingEdited()
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
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(12, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var validator = new EditEntryCommandValidator(context, currentUser);

        // Same/overlapping window as the entry being edited - should not be flagged as a conflict.
        var command = new EditEntryCommand(entryId, Yesterday, new TimeOnly(8, 30), new TimeOnly(12, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NoTimeChange_Fails()
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
        var validator = new EditEntryCommandValidator(context, currentUser);

        // Same start/end time as the existing entry, only the note changes.
        var command = new EditEntryCommand(entryId, Yesterday, new TimeOnly(8, 0), new TimeOnly(16, 0), "Nur ein Kommentar");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Bitte ändern Sie die Kommen- oder Gehen-Zeit, um eine Korrektur einzureichen.");
    }

    [Fact]
    public async Task Validate_NoTimeChange_SubMinutePrecisionFromClockIn_Fails()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var entryId = Guid.NewGuid();
        context.TimeEntries.Add(new TimeEntry
        {
            Id = entryId,
            UserId = userId,
            // Simulates a real clock-in/out with second/millisecond precision.
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(8, 0)).AddSeconds(27).AddMilliseconds(123),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(16, 0)).AddSeconds(5),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var validator = new EditEntryCommandValidator(context, currentUser);

        // The edit form only shows HH:mm (08:00 / 16:00) - resubmitting these "unchanged" values
        // must still be rejected even though the stored entry has second/millisecond precision.
        var command = new EditEntryCommand(entryId, Yesterday, new TimeOnly(8, 0), new TimeOnly(16, 0), "Nur ein Kommentar");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Bitte ändern Sie die Kommen- oder Gehen-Zeit, um eine Korrektur einzureichen.");
    }

    [Fact]
    public async Task Validate_OverlapWithDifferentEntry_Fails()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var otherEntryId = Guid.NewGuid();
        context.TimeEntries.Add(new TimeEntry
        {
            Id = otherEntryId,
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(12, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });

        var editedEntryId = Guid.NewGuid();
        context.TimeEntries.Add(new TimeEntry
        {
            Id = editedEntryId,
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(13, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(15, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var validator = new EditEntryCommandValidator(context, currentUser);

        // Move the edited entry so it overlaps the other entry's 08:00-12:00 window.
        var command = new EditEntryCommand(editedEntryId, Yesterday, new TimeOnly(10, 0), new TimeOnly(14, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Der Zeitraum überlappt mit einem bestehenden Eintrag.");
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
