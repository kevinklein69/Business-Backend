using Business.Domain.Enums;
using FluentValidation;

namespace Business.Application.Features.TimeTracking.UpdateEntryStatus;

public class UpdateEntryStatusCommandValidator : AbstractValidator<UpdateEntryStatusCommand>
{
    public UpdateEntryStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is TimeEntryStatus.Approved or TimeEntryStatus.Rejected)
            .WithMessage("Status muss 'Approved' oder 'Rejected' sein.");
    }
}
