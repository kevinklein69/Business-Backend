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
    string? Department,
    string Street,
    string HouseNumber,
    string Zip,
    string City,
    string? Phone,
    DateOnly EntryDate,
    int? ProbationMonths,
    DateOnly? ProbationEndDate,
    int? VacationDaysEntitlement,
    int? InitialBalanceMinutes,
    decimal? InitialVacationDaysTaken) : IRequest<EmployeeDto>;
