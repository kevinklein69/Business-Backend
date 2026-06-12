using Business.Application.Common.Interfaces;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetBalance;

public class GetBalanceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetBalanceQuery, BalanceDto>
{
    public async Task<BalanceDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        return await TimeEntryQueries.GetBalanceAsync(context, userId, cancellationToken);
    }
}
