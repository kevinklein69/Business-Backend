using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Employees.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.AssignedOrders)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Role = request.Role;
        user.Department = request.Department;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = passwordHasher.Hash(user, request.Password);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new EmployeeDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.Department,
            user.AssignedOrders.Any(o => o.Status != OrderStatus.Done));
    }
}
