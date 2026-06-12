using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.EditEntry;

public class EditEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<EditEntryCommand, TimeEntryDto>
{
    public async Task<TimeEntryDto> Handle(EditEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var entry = await context.TimeEntries
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(TimeEntry), request.Id);

        var newClockIn = TimeTrackingConstants.ToUtc(request.Date, request.StartTime);
        var newClockOut = TimeTrackingConstants.ToUtc(request.Date, request.EndTime);

        entry.IsManual = true;
        entry.Status = TimeEntryStatus.Pending;
        entry.ClockIn = newClockIn;
        entry.ClockOut = newClockOut;
        entry.Note = request.Note;

        await context.SaveChangesAsync(cancellationToken);

        return entry.ToDto();
    }
}
