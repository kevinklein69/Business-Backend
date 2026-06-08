using Business.Application.Common.Interfaces;
using FluentValidation;

namespace Business.Application.Features.Employees.DeleteEmployee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator(ICurrentUserService currentUser)
    {
        RuleFor(x => x.Id)
            .Must(id => id != currentUser.UserId)
            .WithMessage("Sie können Ihr eigenes Konto nicht löschen.");
    }
}
