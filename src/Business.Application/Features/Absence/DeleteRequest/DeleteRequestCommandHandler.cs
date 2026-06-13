using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.DeleteRequest;

public class DeleteRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<DeleteRequestCommand>
{
    public async Task Handle(DeleteRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var absenceRequest = await context.AbsenceRequests
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(AbsenceRequest), request.Id);

        var isOwner = absenceRequest.UserId == userId;
        var isManager = currentUser.Role is "Admin" or "Manager";
        if (!isOwner && !isManager)
            throw new NotFoundException(nameof(AbsenceRequest), request.Id);

        if (absenceRequest.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ConflictException("Der Antrag liegt bereits in der Vergangenheit und kann nicht mehr storniert werden.");

        context.AbsenceRequests.Remove(absenceRequest);
        await context.SaveChangesAsync(cancellationToken);
    }
}
