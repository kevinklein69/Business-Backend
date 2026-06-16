using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.ToggleClock;

public class ToggleClockCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<ToggleClockCommand, ToggleClockResult>
{
    public async Task<ToggleClockResult> Handle(ToggleClockCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var openEntry = await context.TimeEntries
            .Where(t => t.UserId == userId && t.OrderId == null && t.ClockOut == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (openEntry is not null)
        {
            openEntry.ClockOut = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            return new ToggleClockResult(false, null);
        }

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = DateTime.UtcNow,
            ClockOut = null,
            Status = TimeEntryStatus.Approved,
            IsManual = false
        };

        context.TimeEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);

        return new ToggleClockResult(true, entry.ClockIn);
    }
}
