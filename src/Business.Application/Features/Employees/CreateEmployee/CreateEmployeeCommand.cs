using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Employees.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Role Role,
    string? Department) : IRequest<EmployeeDto>;
