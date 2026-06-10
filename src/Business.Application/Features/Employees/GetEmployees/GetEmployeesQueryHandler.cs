using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
{
    public async Task<List<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new EmployeeDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role,
                u.Department,
                u.AssignedOrders.Any(o => o.Status != OrderStatus.Done),
                u.Street,
                u.HouseNumber,
                u.Zip,
                u.City,
                u.Phone,
                u.EntryDate,
                u.ProbationMonths,
                u.ProbationEndDate,
                u.VacationDaysEntitlement))
            .ToListAsync(cancellationToken);
    }
}
