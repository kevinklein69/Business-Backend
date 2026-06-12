using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.CreateManualEntry;

public class CreateManualEntryCommandValidator : AbstractValidator<CreateManualEntryCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateManualEntryCommandValidator(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Bitte geben Sie einen Grund für die Korrektur an.")
            .MaximumLength(TimeTrackingConstants.MaxNoteLength)
            .WithMessage($"Der Grund darf maximal {TimeTrackingConstants.MaxNoteLength} Zeichen lang sein.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("Die Endzeit muss nach der Startzeit liegen.")
            .OverridePropertyName("EndTime");

        RuleFor(x => x)
            .Must(x => TimeTrackingConstants.ToUtc(x.Date, x.EndTime) <= DateTime.UtcNow)
            .WithMessage("Die Zeiterfassung darf nicht in der Zukunft liegen.")
            .OverridePropertyName("Date");

        RuleFor(x => x.Date)
            .Must(IsWithinBackdateLimit)
            .WithMessage($"Mitarbeiter können Einträge nur für die letzten {TimeTrackingConstants.MaxBackdateDays} Tage erfassen.");

        RuleFor(x => x)
            .MustAsync(NotOverlapAsync)
            .WithMessage("Der Zeitraum überlappt mit einem bestehenden Eintrag.")
            .OverridePropertyName("StartTime");
    }

    private bool IsWithinBackdateLimit(DateOnly date) =>
        _currentUser.Role != nameof(Role.Employee)
        || date >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-TimeTrackingConstants.MaxBackdateDays);

    private async Task<bool> NotOverlapAsync(CreateManualEntryCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return true;

        var clockIn = TimeTrackingConstants.ToUtc(command.Date, command.StartTime);
        var clockOut = TimeTrackingConstants.ToUtc(command.Date, command.EndTime);

        return !await _context.TimeEntries
            .Where(t => t.UserId == userId
                     && t.Status != TimeEntryStatus.Rejected
                     && t.ClockOut != null
                     && t.ClockIn < clockOut
                     && t.ClockOut > clockIn)
            .AnyAsync(cancellationToken);
    }
}
