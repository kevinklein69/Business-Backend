using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using Betrieb.Domain.Enums;
using MediatR;

namespace Betrieb.Application.Features.Vacation.CreateRequest;

public class CreateRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<CreateRequestCommand, VacationRequestDto>
{
    public async Task<VacationRequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var vacationRequest = new VacationRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = VacationStatus.Open,
            Comment = request.Comment
        };

        context.VacationRequests.Add(vacationRequest);
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
