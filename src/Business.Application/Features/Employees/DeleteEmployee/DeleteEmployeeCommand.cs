using MediatR;

namespace Business.Application.Features.Employees.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest;
