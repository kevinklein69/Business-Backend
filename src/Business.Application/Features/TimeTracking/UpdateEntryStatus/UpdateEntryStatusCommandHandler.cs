using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.TimeTracking.UpdateEntryStatus;

public class UpdateEntryStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateEntryStatusCommand, TimeEntryDto>
{
    public async Task<TimeEntryDto> Handle(UpdateEntryStatusCommand request, CancellationToken cancellationToken)
    {
        var entry = await context.TimeEntries
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TimeEntry), request.Id);

        entry.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);

        return entry.ToDto();
    }
}
