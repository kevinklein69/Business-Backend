using Business.Domain.Enums;
using FluentValidation;

namespace Business.Application.Features.Absence.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    private static readonly AbsenceType[] AllowedTypes = [AbsenceType.Vacation, AbsenceType.FlexTimeCompensation];

    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Type)
            .Must(t => Array.Exists(AllowedTypes, a => a == t))
            .WithMessage("Bitte wählen Sie eine Urlaubsart aus.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Comment)
            .MaximumLength(500);
    }
}
