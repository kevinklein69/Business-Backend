using Betrieb.Application.Common.Exceptions;
using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using Betrieb.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.Absence.RecordAbsence;

public class RecordAbsenceCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RecordAbsenceCommand, AbsenceRequestDto>
{
    public async Task<AbsenceRequestDto> Handle(RecordAbsenceCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        var absenceRequest = new AbsenceRequest
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = AbsenceStatus.Approved,
            Comment = request.Comment
        };

        context.AbsenceRequests.Add(absenceRequest);
        await context.SaveChangesAsync(cancellationToken);

        absenceRequest.User = user;
        return absenceRequest.ToDto();
    }
}
