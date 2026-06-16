using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.GetClockStatus;

public class GetClockStatusQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetClockStatusQuery, ClockStatusDto>
{
    public async Task<ClockStatusDto> Handle(GetClockStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var openEntry = await context.TimeEntries
            .Where(t => t.UserId == userId && t.OrderId == null && t.ClockOut == null)
            .FirstOrDefaultAsync(cancellationToken);

        return openEntry is null
            ? new ClockStatusDto(false, null)
            : new ClockStatusDto(true, openEntry.ClockIn);
    }
}
