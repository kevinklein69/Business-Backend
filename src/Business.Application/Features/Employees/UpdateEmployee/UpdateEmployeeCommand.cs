using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Employees.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Role Role,
    string? Department,
    string? Password) : IRequest<EmployeeDto>;
