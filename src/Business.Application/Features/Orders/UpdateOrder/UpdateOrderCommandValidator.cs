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

        RuleFor(x => x.Revenue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Revenue.HasValue);

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.ActualHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ActualHours.HasValue);

        RuleFor(x => x.PlannedEndDate)
            .GreaterThanOrEqualTo(x => x.PlannedStartDate!.Value)
            .When(x => x.PlannedStartDate.HasValue && x.PlannedEndDate.HasValue)
            .WithMessage("Das geplante Enddatum darf nicht vor dem Startdatum liegen.");

        RuleFor(x => x.DeviationReason)
            .MaximumLength(500);

        RuleForEach(x => x.Positions).ChildRules(position =>
        {
            position.RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Die Leistung ist erforderlich.")
                .MaximumLength(500);

            position.RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Die Menge muss größer als 0 sein.");

            position.RuleFor(p => p.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Der Einzelpreis darf nicht negativ sein.");
        });
    }
}
