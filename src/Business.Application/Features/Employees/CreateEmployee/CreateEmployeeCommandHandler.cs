using Business.Application.Common;
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
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            Zip = request.Zip,
            City = request.City,
            Phone = request.Phone,
            EntryDate = request.EntryDate,
            ProbationMonths = request.ProbationMonths,
            ProbationEndDate = request.ProbationEndDate ?? ProbationCalculator.CalculateEnd(request.EntryDate, request.ProbationMonths),
            VacationDaysEntitlement = request.VacationDaysEntitlement,
        };
        user.PasswordHash = passwordHasher.Hash(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new EmployeeDto(
            user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Department, false,
            user.Street, user.HouseNumber, user.Zip, user.City, user.Phone,
            user.EntryDate, user.ProbationMonths, user.ProbationEndDate, user.VacationDaysEntitlement);
    }
}
