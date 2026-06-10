using Business.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .MustAsync((command, email, cancellationToken) => IsEmailUniqueAsync(context, command.Id, email, cancellationToken))
            .WithMessage("Diese E-Mail-Adresse wird bereits verwendet.");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.Role)
            .IsInEnum();

        RuleFor(x => x.Department)
            .MaximumLength(100);

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Zip)
            .NotEmpty()
            .Matches(@"^\d{5}$")
            .WithMessage("Die PLZ muss aus 5 Ziffern bestehen.");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.EntryDate)
            .NotEqual(default(DateOnly))
            .WithMessage("Das Eintrittsdatum ist erforderlich.");

        RuleFor(x => x.ProbationMonths)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ProbationMonths.HasValue);

        RuleFor(x => x.VacationDaysEntitlement)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }

    private static Task<bool> IsEmailUniqueAsync(IApplicationDbContext context, Guid id, string email, CancellationToken cancellationToken) =>
        context.Users.AllAsync(u => u.Id == id || u.Email != email, cancellationToken);
}
