using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.UpdateEmployeeRole;

public class UpdateEmployeeRoleCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateEmployeeRoleCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(UpdateEmployeeRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.AssignedOrders)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        user.Role = request.Role;

        await context.SaveChangesAsync(cancellationToken);

        return new EmployeeDto(
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
            user.VacationDaysEntitlement,
            user.InitialBalanceMinutes,
            user.InitialVacationDaysTaken,
            user.InitialVacationYear);
    }
}
