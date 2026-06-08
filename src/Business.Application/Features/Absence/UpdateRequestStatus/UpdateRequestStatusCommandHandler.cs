using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.UpdateRequestStatus;

public class UpdateRequestStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateRequestStatusCommand, AbsenceRequestDto>
{
    public async Task<AbsenceRequestDto> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var absenceRequest = await context.AbsenceRequests
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(AbsenceRequest), request.Id);

        absenceRequest.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);

        return absenceRequest.ToDto(await context.GetCompanyStateAsync(cancellationToken));
    }
}
