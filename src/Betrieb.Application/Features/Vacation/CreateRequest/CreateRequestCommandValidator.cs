using FluentValidation;

namespace Betrieb.Application.Features.Vacation.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Comment)
            .MaximumLength(500);
    }
}
