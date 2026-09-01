using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Employees.GetEmployees;

public record GetEmployeesQuery : IRequest<List<EmployeeDto>>;

public record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Role Role,
    string? Department,
    bool HasActiveOrder,
    string? Street,
    string? HouseNumber,
    string? Zip,
    string? City,
    string? Phone,
    DateOnly? EntryDate,
    int? ProbationMonths,
    DateOnly? ProbationEndDate,
    int? VacationDaysEntitlement,
    int? InitialBalanceMinutes,
    decimal? InitialVacationDaysTaken,
    int? InitialVacationYear);
