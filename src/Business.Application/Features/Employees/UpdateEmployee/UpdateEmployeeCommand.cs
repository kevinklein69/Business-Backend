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
    string? Password,
    string Street,
    string HouseNumber,
    string Zip,
    string City,
    string? Phone,
    DateOnly EntryDate,
    int? ProbationMonths,
    DateOnly? ProbationEndDate,
    int? VacationDaysEntitlement,
    int? InitialBalanceMinutes) : IRequest<EmployeeDto>;
