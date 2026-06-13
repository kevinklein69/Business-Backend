using FluentValidation;

namespace Business.Application.Features.Orders.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Customer)
            .MaximumLength(200);

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Die Straße ist erforderlich.")
            .MaximumLength(200);

        RuleFor(x => x.HouseNumber)
            .NotEmpty().WithMessage("Die Hausnummer ist erforderlich.")
            .MaximumLength(20);

        RuleFor(x => x.Zip)
            .NotEmpty().WithMessage("Die PLZ ist erforderlich.")
            .MaximumLength(10);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Der Ort ist erforderlich.")
            .MaximumLength(100);

        RuleFor(x => x.Revenue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Revenue.HasValue);

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.PlannedEndDate)
            .GreaterThanOrEqualTo(x => x.PlannedStartDate!.Value)
            .When(x => x.PlannedStartDate.HasValue && x.PlannedEndDate.HasValue)
            .WithMessage("Das geplante Enddatum darf nicht vor dem Startdatum liegen.");

        RuleFor(x => x.DeviationReason)
            .MaximumLength(500);
    }
}
