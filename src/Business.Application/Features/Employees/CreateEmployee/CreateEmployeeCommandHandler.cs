using Business.Application.Common.Interfaces;
using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Entities;
using MediatR;

namespace Business.Application.Features.Employees.CreateEmployee;

public class CreateEmployeeCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = request.Role,
            Department = request.Department,
        };
        user.PasswordHash = passwordHasher.Hash(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new EmployeeDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Department, false);
    }
}
