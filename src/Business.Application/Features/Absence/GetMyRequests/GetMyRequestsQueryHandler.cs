using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.GetMyRequests;

public class GetMyRequestsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetMyRequestsQuery, List<AbsenceRequestDto>>
{
    public async Task<List<AbsenceRequestDto>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var requests = await context.AbsenceRequests
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);

        var state = await context.GetCompanyStateAsync(cancellationToken);
        return requests.Select(a => a.ToDto(state)).ToList();
    }
}
