using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.UpdateRequest;

public class UpdateRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<UpdateRequestCommand, AbsenceRequestDto>
{
    public async Task<AbsenceRequestDto> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var absenceRequest = await context.AbsenceRequests
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(AbsenceRequest), request.Id);

        var isOwner = absenceRequest.UserId == userId;
        var isManager = currentUser.Role is "Admin" or "Manager";
        if (!isOwner && !isManager)
            throw new NotFoundException(nameof(AbsenceRequest), request.Id);

        if (absenceRequest.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ConflictException("Der Antrag liegt bereits in der Vergangenheit und kann nicht mehr bearbeitet werden.");

        absenceRequest.Type = request.Type;
        absenceRequest.StartDate = request.StartDate;
        absenceRequest.EndDate = request.EndDate;
        absenceRequest.Comment = request.Comment;
        // Geänderte Anträge müssen erneut geprüft werden, daher zurück auf "Offen".
        absenceRequest.Status = AbsenceStatus.Open;

        await context.SaveChangesAsync(cancellationToken);

        return absenceRequest.ToDto(await context.GetCompanyStateAsync(cancellationToken));
    }
}
