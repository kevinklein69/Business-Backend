using Betrieb.Application.Common.Exceptions;
using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.Vacation.UpdateRequestStatus;

public class UpdateRequestStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateRequestStatusCommand, VacationRequestDto>
{
    public async Task<VacationRequestDto> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var vacationRequest = await context.VacationRequests
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(VacationRequest), request.Id);

        vacationRequest.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);

        return new VacationRequestDto(
            vacationRequest.Id,
            vacationRequest.StartDate,
            vacationRequest.EndDate,
            VacationRequestExtensions.CountBusinessDays(vacationRequest.StartDate, vacationRequest.EndDate),
            vacationRequest.Status,
            vacationRequest.Comment);
    }
}
