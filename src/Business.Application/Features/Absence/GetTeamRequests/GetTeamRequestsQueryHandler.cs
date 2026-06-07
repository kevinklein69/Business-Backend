using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.GetTeamRequests;

public class GetTeamRequestsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTeamRequestsQuery, List<AbsenceRequestDto>>
{
    public async Task<List<AbsenceRequestDto>> Handle(GetTeamRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await context.AbsenceRequests
            .Include(a => a.User)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);

        return requests.Select(a => a.ToDto()).ToList();
    }
}
