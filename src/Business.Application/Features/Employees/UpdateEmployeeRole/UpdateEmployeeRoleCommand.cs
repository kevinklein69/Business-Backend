using Business.Application.Features.Employees.GetEmployees;
using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Employees.UpdateEmployeeRole;

public record UpdateEmployeeRoleCommand(Guid Id, Role Role) : IRequest<EmployeeDto>;
