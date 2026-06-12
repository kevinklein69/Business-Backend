using FluentValidation;

namespace Business.Application.Features.Orders.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Der Titel ist erforderlich.")
            .MaximumLength(200);

        RuleFor(x => x.Customer)
            .NotEmpty().WithMessage("Der Kunde ist erforderlich.")
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

        RuleFor(x => x.PlannedStartDate)
            .NotNull().WithMessage("Das geplante Startdatum ist erforderlich.");

        RuleFor(x => x.PlannedEndDate)
            .NotNull().WithMessage("Das geplante Enddatum ist erforderlich.");

        RuleFor(x => x.AssigneeIds)
            .NotEmpty().WithMessage("Mindestens ein Mitarbeiter muss zugewiesen werden.");

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
