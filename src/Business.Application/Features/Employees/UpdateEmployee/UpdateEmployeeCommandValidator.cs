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
    }

    private static Task<bool> IsEmailUniqueAsync(IApplicationDbContext context, Guid id, string email, CancellationToken cancellationToken) =>
        context.Users.AllAsync(u => u.Id == id || u.Email != email, cancellationToken);
}
