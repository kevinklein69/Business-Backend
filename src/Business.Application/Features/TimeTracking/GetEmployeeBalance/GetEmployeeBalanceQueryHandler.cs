using Business.Application.Common.Interfaces;
using Business.Application.Features.TimeTracking.GetBalance;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetEmployeeBalance;

public class GetEmployeeBalanceQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmployeeBalanceQuery, BalanceDto>
{
    public async Task<BalanceDto> Handle(GetEmployeeBalanceQuery request, CancellationToken cancellationToken)
    {
        return await TimeEntryQueries.GetBalanceAsync(context, request.UserId, cancellationToken);
    }
}
