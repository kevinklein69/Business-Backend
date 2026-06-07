using FluentValidation;

namespace Betrieb.Application.Features.Absence.RecordAbsence;

public class RecordAbsenceCommandValidator : AbstractValidator<RecordAbsenceCommand>
{
    public RecordAbsenceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Comment)
            .MaximumLength(500);
    }
}
