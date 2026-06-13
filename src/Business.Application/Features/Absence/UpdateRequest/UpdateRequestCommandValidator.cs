using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using FluentValidation;

namespace Business.Application.Features.Absence.UpdateRequest;

public class UpdateRequestCommandValidator : AbstractValidator<UpdateRequestCommand>
{
    private static readonly AbsenceType[] SelfServiceTypes = [AbsenceType.Vacation, AbsenceType.FlexTimeCompensation];

    public UpdateRequestCommandValidator(ICurrentUserService currentUser)
    {
        var isManager = currentUser.Role is "Admin" or "Manager";

        RuleFor(x => x.Type)
            .Must(t => isManager || Array.Exists(SelfServiceTypes, a => a == t))
            .WithMessage("Bitte wählen Sie eine Urlaubsart aus.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Comment)
            .MaximumLength(500);
    }
}
