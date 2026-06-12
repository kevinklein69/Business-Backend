using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.ToggleOrderClock;

public class ToggleOrderClockCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<ToggleOrderClockCommand, ToggleOrderClockResult>
{
    public async Task<ToggleOrderClockResult> Handle(ToggleOrderClockCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var orderExists = await context.Orders.AnyAsync(o => o.Id == request.OrderId, cancellationToken);
        if (!orderExists)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var openEntry = await context.TimeEntries
            .Where(t => t.UserId == userId && t.OrderId == request.OrderId && t.ClockOut == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (openEntry is not null)
        {
            openEntry.ClockOut = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            await TimeEntryQueries.RecalculateOrderActualHoursAsync(context, request.OrderId, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new ToggleOrderClockResult(false, null);
        }

        // Only one order-clock can be active at a time: close any other open order entry first.
        var otherOpenEntry = await context.TimeEntries
            .Where(t => t.UserId == userId && t.OrderId != null && t.ClockOut == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (otherOpenEntry is not null)
        {
            otherOpenEntry.ClockOut = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            await TimeEntryQueries.RecalculateOrderActualHoursAsync(context, otherOpenEntry.OrderId!.Value, cancellationToken);
        }

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = request.OrderId,
            ClockIn = DateTime.UtcNow,
            ClockOut = null,
            Status = TimeEntryStatus.Approved,
            IsManual = false
        };

        context.TimeEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);

        return new ToggleOrderClockResult(true, entry.ClockIn);
    }
}
