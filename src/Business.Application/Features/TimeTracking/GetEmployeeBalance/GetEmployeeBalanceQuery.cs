using Business.Application.Features.TimeTracking.GetBalance;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetEmployeeBalance;

public record GetEmployeeBalanceQuery(Guid UserId) : IRequest<BalanceDto>;
