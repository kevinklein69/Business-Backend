using Business.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(IApplicationDbContext context)
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
            .MustAsync((email, cancellationToken) => IsEmailUniqueAsync(context, email, cancellationToken))
            .WithMessage("Diese E-Mail-Adresse wird bereits verwendet.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Role)
            .IsInEnum();

        RuleFor(x => x.Department)
            .MaximumLength(100);
    }

    private static Task<bool> IsEmailUniqueAsync(IApplicationDbContext context, string email, CancellationToken cancellationToken) =>
        context.Users.AllAsync(u => u.Email != email, cancellationToken);
}
