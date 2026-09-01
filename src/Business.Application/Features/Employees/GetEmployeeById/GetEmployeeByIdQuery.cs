using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Employees.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDetailDto>;

public record EmployeeDetailDto(
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
    int? InitialVacationYear,
    decimal RemainingVacationDays,
    decimal SickDaysThisYear);

public static class EmployeeDetailDtoExtensions
{
    public static EmployeeDetailDto ToDetailDto(this EmployeeDto employee, decimal remainingVacationDays, decimal sickDaysThisYear) =>
        new(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Role,
            employee.Department,
            employee.HasActiveOrder,
            employee.Street,
            employee.HouseNumber,
            employee.Zip,
            employee.City,
            employee.Phone,
            employee.EntryDate,
            employee.ProbationMonths,
            employee.ProbationEndDate,
            employee.VacationDaysEntitlement,
            employee.InitialBalanceMinutes,
            employee.InitialVacationDaysTaken,
            employee.InitialVacationYear,
            remainingVacationDays,
            sickDaysThisYear);
}
