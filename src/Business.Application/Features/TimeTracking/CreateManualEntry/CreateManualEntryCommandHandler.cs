using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.CreateManualEntry;

public class CreateManualEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<CreateManualEntryCommand, TimeEntryDto>
{
    public async Task<TimeEntryDto> Handle(CreateManualEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClockIn = TimeTrackingConstants.ToUtc(request.Date, request.StartTime),
            ClockOut = TimeTrackingConstants.ToUtc(request.Date, request.EndTime),
            Status = TimeEntryStatus.Pending,
            IsManual = true,
            Note = request.Note
        };

        context.TimeEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);

        var saved = await context.TimeEntries
            .FirstAsync(t => t.Id == entry.Id, cancellationToken);

        return saved.ToDto();
    }
}
