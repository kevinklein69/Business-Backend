using Betrieb.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.Vacation.GetMyRequests;

public class GetMyRequestsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetMyRequestsQuery, List<VacationRequestDto>>
{
    public async Task<List<VacationRequestDto>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var requests = await context.VacationRequests
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.StartDate)
            .ToListAsync(cancellationToken);

        return requests
            .Select(v => new VacationRequestDto(
                v.Id,
                v.StartDate,
                v.EndDate,
                VacationRequestExtensions.CountBusinessDays(v.StartDate, v.EndDate),
                v.Status,
                v.Comment))
            .ToList();
    }
}
