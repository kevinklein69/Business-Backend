using Business.Application.Features.TimeTracking;
using Business.Application.Features.TimeTracking.CreateManualEntry;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.Infrastructure.Persistence;

namespace Business.UnitTests.TimeTracking;

public class CreateManualEntryCommandValidatorTests
{
    private static readonly DateOnly Yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

    private static CreateManualEntryCommand ValidCommand(DateOnly? date = null) =>
        new(date ?? Yesterday, new TimeOnly(9, 0), new TimeOnly(17, 0), "Vergessen einzustempeln");

    [Fact]
    public async Task Validate_EndTimeBeforeStartTime_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(17, 0), new TimeOnly(9, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Die Endzeit muss nach der Startzeit liegen.");
    }

    [Fact]
    public async Task Validate_EndTimeEqualsStartTime_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(9, 0), new TimeOnly(9, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Die Endzeit muss nach der Startzeit liegen.");
    }

    [Fact]
    public async Task Validate_FutureDate_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        var command = ValidCommand(tomorrow);

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Die Zeiterfassung darf nicht in der Zukunft liegen.");
    }

    [Fact]
    public async Task Validate_EmployeeBackdatingBeyond30Days_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var tooOld = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-(TimeTrackingConstants.MaxBackdateDays + 1));
        var command = ValidCommand(tooOld);

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("letzten 30 Tage"));
    }

    [Fact]
    public async Task Validate_AdminBackdatingBeyond30Days_Succeeds()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Admin) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var tooOldForEmployee = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-(TimeTrackingConstants.MaxBackdateDays + 30));
        var command = ValidCommand(tooOldForEmployee);

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_OverlappingApprovedEntry_Fails()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(12, 0)),
            Status = TimeEntryStatus.Approved,
            IsManual = false
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        // Overlaps the existing 08:00-12:00 entry.
        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(10, 0), new TimeOnly(14, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Der Zeitraum überlappt mit einem bestehenden Eintrag.");
    }

    [Fact]
    public async Task Validate_OverlappingRejectedEntry_Succeeds()
    {
        var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        context.TimeEntries.Add(new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(8, 0)),
            ClockOut = TimeTrackingConstants.ToUtc(Yesterday, new TimeOnly(12, 0)),
            Status = TimeEntryStatus.Rejected,
            IsManual = true,
            Note = "Abgelehnt"
        });
        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = userId, Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(10, 0), new TimeOnly(14, 0), "Korrektur");

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyNote_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(9, 0), new TimeOnly(17, 0), "");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Bitte geben Sie einen Grund für die Korrektur an.");
    }

    [Fact]
    public async Task Validate_NoteExceedsMaxLength_Fails()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var command = new CreateManualEntryCommand(Yesterday, new TimeOnly(9, 0), new TimeOnly(17, 0), new string('x', TimeTrackingConstants.MaxNoteLength + 1));

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("maximal"));
    }

    [Fact]
    public async Task Validate_ValidCommand_Succeeds()
    {
        var context = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService { UserId = Guid.NewGuid(), Role = nameof(Role.Employee) };
        var validator = new CreateManualEntryCommandValidator(context, currentUser);

        var result = await validator.ValidateAsync(ValidCommand());

        Assert.True(result.IsValid);
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
