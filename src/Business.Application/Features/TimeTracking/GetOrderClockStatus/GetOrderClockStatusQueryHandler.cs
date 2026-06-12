using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.GetOrderClockStatus;

public class GetOrderClockStatusQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetOrderClockStatusQuery, OrderClockStatusDto>
{
    public async Task<OrderClockStatusDto> Handle(GetOrderClockStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var openEntry = await context.TimeEntries
            .Where(t => t.UserId == userId && t.OrderId == request.OrderId && t.ClockOut == null)
            .FirstOrDefaultAsync(cancellationToken);

        return openEntry is null
            ? new OrderClockStatusDto(false, null)
            : new OrderClockStatusDto(true, openEntry.ClockIn);
    }
}
