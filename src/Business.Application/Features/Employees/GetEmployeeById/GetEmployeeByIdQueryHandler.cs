using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Application.Features.Absence;
using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailDto>
{
    public async Task<EmployeeDetailDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.AssignedOrders)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        var employee = new EmployeeDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.Department,
            user.AssignedOrders.Any(o => o.Status != OrderStatus.Done),
            user.Street,
            user.HouseNumber,
            user.Zip,
            user.City,
            user.Phone,
            user.EntryDate,
            user.ProbationMonths,
            user.ProbationEndDate,
            user.VacationDaysEntitlement);

        var state = await context.GetCompanyStateAsync(cancellationToken);
        var currentYear = DateTime.UtcNow.Year;

        var approvedAbsences = await context.AbsenceRequests
            .Where(a => a.UserId == user.Id && a.Status == AbsenceStatus.Approved && a.StartDate.Year == currentYear)
            .ToListAsync(cancellationToken);

        var vacationTaken = approvedAbsences
            .Where(a => a.Type == AbsenceType.Vacation)
            .Sum(a => AbsenceRequestExtensions.CountBusinessDays(a.StartDate, a.EndDate, state));

        var sickDays = approvedAbsences
            .Where(a => a.Type == AbsenceType.Sick)
            .Sum(a => AbsenceRequestExtensions.CountBusinessDays(a.StartDate, a.EndDate, state));

        var remainingVacationDays = (employee.VacationDaysEntitlement ?? 0) - vacationTaken;

        return employee.ToDetailDto(remainingVacationDays, sickDays);
    }
}
